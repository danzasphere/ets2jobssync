using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;


namespace Palow.Library.Config {
    public class ConfigXmlFile {

        #region Properties
        private String fileSource;
        /// <summary>
        /// Pfad der XML Datei
        /// </summary>
        public String FileSource {
            get { return fileSource; }
            set { fileSource = value; }
        }

        private string fileContent { get; set; }

        public string Version { get; set; }

        private string _objectTypeText;
        public string ObjectType {
            get {
                return _objectTypeText;
            }
            set {
                _objectTypeText = value;
            }
        }

        private string _objectSubTypeText;
        public string ObjectSubType {
            get {
                return _objectSubTypeText;
            }
            set {
                _objectSubTypeText = value;
            }
        }

        Object objectHolder;
        public Object ObjectHolder {
            get { return objectHolder; }
            set { objectHolder = value; }
        }



        #endregion

        #region Konstruktor
        public ConfigXmlFile(String fileSource, Boolean AutoLoad) {
            this.FileSource = fileSource;
            if (AutoLoad) {
                Load();
            }
        }
        public ConfigXmlFile(String fileSource) : this(fileSource, false) { }

        #endregion

        #region Method
        public GENTYP GetObject<GENTYP>() {
            ILog lg = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            MemoryStream stream = null;
            try {
                // FileContent -> Replace "<ObjectStore>";

                string xmlstr = this.fileContent.Replace("<ObjectStore", "<" + typeof(GENTYP).Name);
                xmlstr = xmlstr.Replace("</ObjectStore", "</" + typeof(GENTYP).Name);

                XmlSerializer ser = new XmlSerializer(typeof(GENTYP));
                stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(xmlstr));

                this.ObjectHolder = ser.Deserialize(stream);

                if (stream != null) {
                    stream.Close();
                }
                lg.Debug("Returning Type: " + typeof(GENTYP).Name);
            } catch (Exception ex) {
                lg.Error(Functions.GetExceptionText(ex));
                throw new Exception("Error in XML Deserilization.", ex);
            } finally {
                //Am ende noch den FileStream schliessen. 
                if (stream != null) {
                    stream.Close();
                }
            }
            return (GENTYP)this.objectHolder;
        }
        /// <summary>

        public ConfigClass Get<ConfigClass>() where ConfigClass : IConfig, new() {
            ILog lg = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            if (!LoadExecuted) Load();
            ConfigClass akt = new ConfigClass();

            try {
                akt = this.GetObject<ConfigClass>();
                akt.IsCorrectlyLoaded = true;
                return akt;
            } catch (Exception ex) {
                string ertx = string.Format("Config File '{0}' not loaded, error extracting settings.", this.FileSource);
                lg.Error(Functions.GetExceptionText(ex));
                throw new Exception(ertx, ex);
            }            
        }


        private Boolean LoadExecuted = false;
        /// <summary>
        /// Lädt File ohne Konstruktor (wenn FileSource in der Klasse angegeben wurde)
        /// </summary>
        /// <returns></returns>
        public void Load() {
            ILog lg = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            //try {
            this.fileContent = ReadFile(this.fileSource);
            // **** ANALYSE CONTENT
            XmlDocument xdoc = new XmlDocument();
            MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(this.fileContent));
            xdoc.Load(stream);
            XmlNode xobj = xdoc.FirstChild;
            xobj = xobj.NextSibling;
            //lg.Debug("Object: " + xobj.Name);
            //this.ObjectType = xobj.Name;

            XmlNodeList xNdList = xobj.ChildNodes;
            foreach (XmlNode xNode in xNdList) {
                if (xNode.InnerText != null) {
                    // lg.Debug("'" + xNode.Name + "' = '" + xNode.InnerText + "'");
                    if (xNode.Name == "Version") this.Version = xNode.InnerText;
                    if (xNode.Name == "ObjectType") this.ObjectType = xNode.InnerText;
                    if (xNode.Name == "ObjectSubType") this.ObjectSubType = xNode.InnerText;
                }
            }
            lg.Debug("Loading Ready: " + this.ObjectType.ToString() + "." + this.ObjectSubType.ToString() + " Version:" + this.Version);

            //} catch (Exception ex) {
            //    res.MessageError = Functions.GetExceptionText(ex, false);
            //    lg.Error(Functions.GetExceptionText(ex));
            //    throw new Exception(ex);
            //}
            LoadExecuted = true;

        }
        private string ReadFile(String sFilename) {
            string sContent = "";

            if (File.Exists(sFilename)) {
                StreamReader myFile = new StreamReader(sFilename, System.Text.Encoding.Default);
                sContent = myFile.ReadToEnd();
                myFile.Close();
            }
            return sContent;
        }

        /// <summary>
        /// Speichert die Datei in XML
        /// </summary>
        /// <param name="o"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Boolean Save(Object o) {
            ILog lg = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            XmlSerializer ser = null;
            //FileStream str = null;
            lg.Debug("Got ObjectType: " + o.GetType().Name);


            try {
                ser = new XmlSerializer(o.GetType());
                MemoryStream memstream = new MemoryStream();
                ser.Serialize(memstream, o);
                memstream.Position = 0;
                StreamReader reader = new StreamReader(memstream);
                string text = reader.ReadToEnd();

                text = text.Replace("<" + o.GetType().Name, "<ObjectStore");
                text = text.Replace("</" + o.GetType().Name, "</ObjectStore");

                WriteFile(this.fileSource, text);
            } catch (Exception ex) {
                lg.Error(Functions.GetExceptionText(ex));
                throw new Exception("Error Saving XmlFile", ex);

            }

            return true;
        }

        public Boolean SaveAs(Object o, string NewFileName) {
            ILog lg = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            lg.Debug("Got ObjectType: " + o.GetType().Name);
            lg.Debug("Start SaveAs:");
            lg.Debug("Old Filename: '" + this.FileSource + "'");
            lg.Debug("New Filename: '" + NewFileName + "'");
            Boolean res = false;
            System.IO.File.Delete(this.FileSource);
            lg.Debug("Old File Deleted.");

            this.fileSource = NewFileName;
            Save(o);

            return res;
        }

        ///<summary>
        /// Schreibt den übergebenen Inhalt in eine Textdatei.
        ///</summary>
        ///<param name="sFilename">Pfad zur Datei</param>
        ///<param name="sLines">zu schreibender Text</param>
        private void WriteFile(String sFilename, String sLines) {
            StreamWriter myFile = new StreamWriter(sFilename);
            myFile.Write(sLines);
            myFile.Close();
        }

        #endregion


    }
}
