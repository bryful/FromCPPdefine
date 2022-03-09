using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FromCPPdefine
{
	public class FromCPPdefine
	{
		/*
		 *
		 public static class Define
		{
			// static readonlyの場合
			public static readonly int ST_MAX_STRING = 256;
			public static readonly string ST_ERROR_MSG = "error!"; 
			// constの場合
			public const int CS_MAX_STRING = 256;
			public const string CS_ERROR_MSG = "error!";
		}
		*/
		// *********************************************************************
		private string m_path = "";
		private defineline[] m_di = new defineline[0];
		// *********************************************************************
		public class defineline
		{
			public string name = "";
			public string value = "";
			public string comment = "";

			public string Info()
			{
				string ret =  name + "=" + value;
				if (comment!="")
				{
					ret += "//" + comment;
				}
				return ret;
			}
			public string InfoCS()
			{
				string ret = "";
				ret += "public static readonly ";
				if (value[0] == '"')
				{
					ret += "string ";
				}
				else
				{
					ret += "int ";

				}
				ret += name + " = " + value + ";";
				return ret;
			}
		}
		// *********************************************************************
		public FromCPPdefine()
		{
		}
		// *********************************************************************
		public FromCPPdefine (string s)
		{
			loadFile(s);
		}
		// *********************************************************************
		public string DispDL(defineline[] dl)
		{
			string ret = "";
			if (dl.Length>0)
			{
				for (int i=0; i<dl.Length;i++)
				{
					if (ret != "") ret += "\r\n";
					ret += dl[i].Info();
				}
			}
			return ret;
		}
		// *********************************************************************
		public string Disp()
		{
			return DispDL(m_di);
		}
		// *********************************************************************
		public string ToCSHARP(string nm)
		{
			string ret = "public static class "+nm+"\r\n";
			ret += "{\r\n";
			if (m_di.Length > 0)
			{
				for (int i = 0; i < m_di.Length; i++)
				{
					if (ret != "") ret += "\r\n";
					ret += "\t" + m_di[i].InfoCS();
				}
			}
			ret += "\r\n}\r\n";

			return ret;
		}
		// *********************************************************************
		static private string splitDefine(string s)
		{
			string ret = "";
			s = s.Trim();
			s = s.Replace("\t", " ");

			int len = s.Length;
			int idx = 0;
			while(idx<len)
			{
				if (s[idx]==' ')
				{
					if (s[idx + 1] == ' ')
					{
						idx++;
						continue;
					}
				}
				ret += s[idx];
				idx++;
			}
			return ret;
		}
		// *********************************************************************
		static public List<defineline> change(string s)
		{
			List<defineline> ret = new List<defineline>();
			if (s == String.Empty) return ret;
			string[] sa = s.Split('\n');
			if (sa.Length <= 0) return ret;

			for (int i=0; i< sa.Length; i++)
			{
				string line = splitDefine(sa[i]);
				if (line.IndexOf("#define ")==0)
				{
					defineline ld = new defineline();

					int idx0 = line.IndexOf("//");
					string line2 = "";
					if (idx0>=0)
					{
						ld.comment = line.Substring(idx0+2).Trim();
						line2 = line.Substring(0, idx0);
					}
					else
					{
						line2 = line;
					}

					int idx1 = line2.IndexOf(" ");
					if (idx1 > 0)
					{
						int idx2 = line2.IndexOf(" ", idx1 + 1);
						if (idx2>idx1)
						{
							ld.name = line2.Substring(idx1, idx2 - idx1).Trim();
							ld.value = line2.Substring(idx2).Trim();
							if((ld.name[ld.name.Length-1]!=')')&& (ld.value[ld.value.Length - 1] != '\\'))
							{
								ret.Add(ld);
							}
						}
					}
				}
			}

			return ret;
		}
		// *********************************************************************
		public  bool loadFile(string s)
		{
			bool ret = false;

			if (File.Exists(s) == false) return ret;

			try
			{
				string str = File.ReadAllText(s, Encoding.GetEncoding("shift_jis"));

				m_di = change(str).ToArray();
			}
			catch
			{
				m_di = new defineline[0];
				ret = false;
			}
			return ret;
		}
		// *********************************************************************
		public string changeExec(string s)
		{
			m_di = new defineline[0];
			m_di = change(s).ToArray();

			return Disp();
		}
	}
}
