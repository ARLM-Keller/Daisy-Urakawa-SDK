using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

namespace Z3986ToXUK
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class MainClass
	{
		const string USAGE = "Usage:\tZ3986ToXUK -dtbook:<source> -output:<dest>";
    static string dtbook;
    static string output;

    static string GetExeDir()
    {
      return System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
    }

    
    
    /// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
      if (!ParseCommandLineArguments(args))
      {
        Console.WriteLine(USAGE); 
        return -1;
      }
      try
      {
        dtbook = Path.Combine(Directory.GetCurrentDirectory(), dtbook);
        output = Path.Combine(Directory.GetCurrentDirectory(), output);

				XmlInstanceGenerator gen = new XmlInstanceGenerator(dtbook);
        gen.Progress += new XmlInstanceGeneratorProgressEventDelegate(gen_Progress);
        XmlDocument instanceDoc = gen.GenerateInstanceXml(true, true);
        XmlTextWriter wr = new XmlTextWriter(output, System.Text.Encoding.UTF8);
        wr.Indentation = 1;
        wr.IndentChar = ' ';
        wr.Formatting = Formatting.Indented;
        instanceDoc.WriteTo(wr);
        wr.Close();
      }
      catch (Exception e)
      {
        Console.WriteLine(
          "An exception occured:\n{0}\nStack Trace:\n{1}",
          e.Message, e.StackTrace);
        while (e.InnerException!=null)
        {
          e = e.InnerException;
          Console.WriteLine(
            "Exception {0}:\n{1}", e.GetType().ToString(), e.Message);
        }
        return -1;
      }

      return 0;
		}

    static bool ParseArgument(string arg, out string name, out string val)
    {
      if (arg.StartsWith("-"))
      {
        string[] parts = arg.Substring(1).Split(new char[] {':'});
        if (parts.Length>1)
        {
          name = parts[0];
          val = parts[1];
          for (int i=2; i<parts.Length; i++)
          {
            val += ":"+parts[i];
          }
          return true;
        }
      }
      name = null; 
      val = null;
      return false;
    }

    static bool ParseCommandLineArguments(string[] args)
    {
      string name, val;
      foreach (string arg in args)
      {
        if (ParseArgument(arg, out name, out val))
        {
          switch (name.ToLower())
          {
            case "dtbook":
              dtbook = val;
              break;
            case "output":
              output = val;
              break;
            default:
              Console.WriteLine("Invalid argument {0}", arg);
              return false;
          }
        }
        else
        {
          Console.WriteLine("Invalid argument {0}", arg);
          return false;
        }
      }
      if (dtbook==null)
      {
        Console.WriteLine("Enter dtbook file path:");
        dtbook = Console.ReadLine().Trim();
      }
      if (dtbook=="")
      {
        Console.WriteLine("No input dtbook file was given");
        return false;
      }
      if (output==null)
      {
        Console.WriteLine("Enter output xuk path:");
        string defOutput = Path.Combine(
          Path.GetDirectoryName(dtbook),
          Path.GetFileNameWithoutExtension(dtbook)+".xuk");
        Console.WriteLine("(Press Enter for default output {0})", defOutput);
        output = Console.ReadLine().Trim();
        if (output=="") output = defOutput;
      }
      if (output=="")
      {
        Console.WriteLine("No output file was given");
        return false;
      }
      return true;
    }

    private static void gen_Progress(XmlInstanceGenerator o, ProgressEventArgs e)
    {
      Console.WriteLine(e.Message);
    }
  }
}
