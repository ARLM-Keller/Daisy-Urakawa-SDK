﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using urakawa.data;
using urakawa.xuk;
namespace urakawa.ExternalFiles
    {
    public class XSLTExternalFileData:ExternalFileData
        {


        public override string MimeType
            {
            get
                {
                return DataProviderFactory.STYLE_CSS_MIME_TYPE;
                }
            }

        public override string GetTypeNameFormatted ()
            {
            return XukStrings.XSLTExternalFileData ;
            }

        }
    }