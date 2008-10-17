using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using urakawa.media.timing;

namespace urakawa.media.data.audio
{
    /// <summary>
    /// Represents information describing raw PCM data
    /// </summary>
    public class PCMDataInfo : PCMFormatInfo, IValueEquatable<PCMDataInfo>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public PCMDataInfo()
            : base()
        {
            DataLength = 0;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other">The PCMDataInfo to copy</param>
        public PCMDataInfo(PCMDataInfo other)
            : base(other)
        {
            DataLength = other.DataLength;
        }

        /// <summary>
        /// Copy constructor copying from a <see cref="PCMFormatInfo"/>, using the default value for data length
        /// </summary>
        /// <param name="other">The PCMFormatInfo to copy from</param>
        public PCMDataInfo(PCMFormatInfo other)
            : base(other)
        {
            DataLength = 0;
        }

        private uint mDataLength = 0;

        /// <summary>
        /// Gets the count in bytes of the raw PCM data
        /// </summary>
        public uint DataLength
        {
            get { return mDataLength; }
            set { mDataLength = value; }
        }

        /// <summary>
        /// Gets the duration of the RAW PCM data
        /// </summary>
        /// <returns>The duration as a <see cref="TimeSpan"/></returns>
        public TimeDelta Duration
        {
            get { return GetDuration(DataLength); }
        }

        /// <summary>
        /// Writes a RIFF Wave PCM header to a given destination output <see cref="Stream"/>
        /// </summary>
        /// <param name="output">The destination output <see cref="Stream"/></param>
        public void WriteRiffWaveHeader(Stream output)
        {
            BinaryWriter wr = new BinaryWriter(output);
            wr.Write(Encoding.ASCII.GetBytes("RIFF")); //Chunk Uid
            uint chunkSize = 4 + 8 + 16 + 8 + DataLength;
            wr.Write(DataLength + 4 + 8 + 16 + 8); //Chunk Size
            wr.Write(Encoding.ASCII.GetBytes("WAVE")); //Format field
            wr.Write(Encoding.ASCII.GetBytes("fmt ")); //Format sub-chunk
            uint formatChunkSize = 16;
            wr.Write(formatChunkSize);
            ushort audioFormat = 1; //PCM format
            wr.Write(audioFormat);
            wr.Write(NumberOfChannels);
            wr.Write(SampleRate);
            wr.Write(ByteRate);
            wr.Write(BlockAlign);
            wr.Write(BitDepth);
            wr.Write(Encoding.ASCII.GetBytes("data"));
            wr.Write(DataLength);
        }

        /// <summary>
        /// Parses a RIFF WAVE PCM header of a given input <see cref="Stream"/>
        /// </summary>
        /// <remarks>
        /// Upon succesful parsing the <paramref name="input"/> <see cref="Stream"/> is positioned at the beginning of the actual PCM data,
        /// that is at the beginning of the data field of the data sub-chunk
        /// </remarks>
        /// <param name="input">The input <see cref="Stream"/> - must be positioned at the start of the RIFF chunk</param>
        /// <returns>A <see cref="PCMDataInfo"/> containing the parsed data</returns>
        /// <exception cref="exception.InvalidDataFormatException">
        /// Thrown when RIFF WAVE header is invalid or is not PCM data
        /// </exception>
        public static PCMDataInfo ParseRiffWaveHeader(Stream input)
        {
            BinaryReader rd = new BinaryReader(input);
            if (input.Length - input.Position < 12)
            {
                throw new exception.InvalidDataFormatException(
                    "The RIFF chunk descriptor does not fit in the input stream");
            }
            string chunkId = Encoding.ASCII.GetString(rd.ReadBytes(4));
            if (chunkId != "RIFF")
            {
                throw new exception.InvalidDataFormatException("ChunkId is not RIFF");
            }
            uint chunkSize = rd.ReadUInt32();
            long chunkEndPos = input.Position + chunkSize;
            if (chunkEndPos > input.Length)
            {
                throw new exception.InvalidDataFormatException(String.Format(
                                                                   "The WAVE PCM chunk does not fit in the input Stream (expected chunk end position is {0:0}, Stream count is {1:0})",
                                                                   chunkEndPos, input.Length));
            }
            string format = Encoding.ASCII.GetString(rd.ReadBytes(4));
            if (format != "WAVE")
            {
                throw new exception.InvalidDataFormatException(String.Format(
                                                                   "RIFF format {0} is not supported. The only supported RIFF format is WAVE",
                                                                   format));
            }
            bool foundFormatSubChunk = false;
            PCMDataInfo pcmInfo = new PCMDataInfo();
            // Search for format subchunk
            while (input.Position + 8 <= chunkEndPos)
            {
                string formatSubChunkId = Encoding.ASCII.GetString(rd.ReadBytes(4));
                uint formatSubChunkSize = rd.ReadUInt32();
                if (input.Position + formatSubChunkSize > chunkEndPos)
                {
                    throw new exception.InvalidDataFormatException(String.Format(
                                                                       "ChunkId {0} does not fit in RIFF chunk",
                                                                       formatSubChunkId));
                }
                if (formatSubChunkId == "fmt ")
                {
                    foundFormatSubChunk = true;
                    if (formatSubChunkSize < 2)
                    {
                        throw new exception.InvalidDataFormatException(
                            "No room for AudioFormat field in format sub-chunk");
                    }
                    ushort audioFormat = rd.ReadUInt16();
                    if (audioFormat != 1)
                    {
                        throw new exception.InvalidDataFormatException(String.Format(
                                                                           "AudioFormat is not PCM (AudioFormat is {0:0})",
                                                                           audioFormat));
                    }
                    if (formatSubChunkSize != 16)
                    {
                        throw new exception.InvalidDataFormatException(String.Format(
                                                                           "Invalid format sub-chink size {0:0} for PCM - must be 16 bytes",
                                                                           audioFormat));
                    }
                    ushort numChannels = rd.ReadUInt16();
                    if (numChannels == 0)
                    {
                        throw new exception.InvalidDataFormatException("0 channels of audio is not supported");
                    }
                    uint sampleRate = rd.ReadUInt32();
                    uint byteRate = rd.ReadUInt32();
                    ushort blockAlign = rd.ReadUInt16();
                    ushort bitDepth = rd.ReadUInt16();
                    if ((bitDepth%8) != 0)
                    {
                        throw new exception.InvalidDataFormatException(String.Format(
                                                                           "Invalid number of bits per sample {0:0} - must be a mulitpla of 8",
                                                                           bitDepth));
                    }
                    if (blockAlign != (numChannels*bitDepth/8))
                    {
                        throw new exception.InvalidDataFormatException(String.Format(
                                                                           "Invalid block align {0:0} - expected {1:0}",
                                                                           blockAlign, numChannels*bitDepth/8));
                    }
                    if (byteRate != sampleRate*blockAlign)
                    {
                        throw new exception.InvalidDataFormatException(String.Format(
                                                                           "Invalid byte rate {0:0} - expected {1:0}",
                                                                           byteRate, sampleRate*blockAlign));
                    }
                    pcmInfo.BitDepth = bitDepth;
                    pcmInfo.NumberOfChannels = numChannels;
                    pcmInfo.SampleRate = sampleRate;
                    break;
                }
                else
                {
                    input.Seek(formatSubChunkSize, SeekOrigin.Current);
                }
            }
            if (!foundFormatSubChunk)
            {
                throw new exception.InvalidDataFormatException("Found no format sub-chunk");
            }
            bool foundDataSubChunk = false;
            while (input.Position + 8 <= chunkEndPos)
            {
                string dataSubChunkId = Encoding.ASCII.GetString(rd.ReadBytes(4));
                uint dataSubChunkSize = rd.ReadUInt32();
                if (input.Position + dataSubChunkSize > chunkEndPos)
                {
                    throw new exception.InvalidDataFormatException(String.Format(
                                                                       "ChunkId {0} does not fit in RIFF chunk",
                                                                       dataSubChunkId));
                }
                if (dataSubChunkId == "data")
                {
                    foundDataSubChunk = true;
                    pcmInfo.DataLength = dataSubChunkSize;
                    break;
                }
                else
                {
                    input.Seek(dataSubChunkSize, SeekOrigin.Current);
                }
            }
            if (!foundDataSubChunk)
            {
                throw new exception.InvalidDataFormatException("Found no data sub-chunk");
            }
            return pcmInfo;
        }


        /// <summary>
        /// Compares the data in two data streams for equality
        /// </summary>
        /// <param name="s1">The first </param>
        /// <param name="s2"></param>
        /// <param name="length">The length of the data to compare</param>
        /// <returns>A <see cref="bool"/> indicating data equality</returns>
        public static bool CompareStreamData(Stream s1, Stream s2, int length)
        {
            byte[] d1 = new byte[length];
            byte[] d2 = new byte[length];
            if (s1.Read(d1, 0, length) != length) return false;
            if (s2.Read(d2, 0, length) != length) return false;
            for (int i = 0; i < length; i++)
            {
                if (d1[i] != d2[i]) return false;
            }
            return true;
        }

        #region IXukAble Members

        /// <summary>
        /// Reads the attributes of a PCMDataInfo xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="System.Xml.XmlReader"/></param>
        protected override void XukInAttributes(System.Xml.XmlReader source)
        {
            base.XukInAttributes(source);
            string attr = source.GetAttribute("dataLength");
            if (attr == null)
            {
                throw new exception.XukException("Attribute DataLength is missing");
            }
            uint dl;
            if (!UInt32.TryParse(attr, out dl))
            {
                throw new exception.XukException(String.Format(
                                                     "Attribute DataLength value {0} is not an unsigned integer",
                                                     attr));
            }
            DataLength = dl;
        }

        /// <summary>
        /// Writes the attributes of a PCMDataInfo element
        /// </summary>
        /// <param name="destination">The destination <see cref="System.Xml.XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(System.Xml.XmlWriter destination, Uri baseUri)
        {
            destination.WriteAttributeString("dataLength", DataLength.ToString());
            base.XukOutAttributes(destination, baseUri);
        }

        #endregion

        #region IValueEquatable<PCMDataInfo> Members

        /// <summary>
        /// Determines if <c>this</c> has the same value as a given other <see cref="PCMDataInfo"/>
        /// </summary>
        /// <param name="other">The given other PCMDataInfo with which to compare</param>
        /// <returns>A <see cref="bool"/> indicating value equality</returns>
        public bool ValueEquals(PCMDataInfo other)
        {
            if (!base.ValueEquals(other)) return false;
            if (DataLength != other.DataLength) return false;
            return true;
        }

        #endregion
    }
}