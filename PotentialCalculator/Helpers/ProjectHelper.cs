using PotentialCalculator.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PotentialCalculator.Helpers {
    public static class ProjectHelper {
        private const string STR_Criterias = "Criterias";
        private const string STR_Value = "Value";
        private const string STR_Type = "Type";
        private const string STR_ProjectSettings = "ProjectSettings";
        private const string STR_CCThreshold = "CCThreshold";
        public static void Open(Project project, string file) {
            XDocument doc = XDocument.Load(file);
            XElement root = doc.Root;
            XElement projectSettings = root.Element(STR_ProjectSettings);
            project.CCThreshold = (double)ReadElement(projectSettings, STR_CCThreshold);
            XElement criterias = root.Element(STR_Criterias);
            foreach (XNode plugin in criterias.Nodes()) {
                XElement element = (XElement)plugin;
                string key = element.Name.LocalName;
                Dictionary<string, object> list = new Dictionary<string, object>();
                ReadProperties(element, list);
                project.Criterias.Add(new Criteria() {
                    SourceMean = (double)list[nameof(Criteria.SourceMean)],
                    SourceSigma = (double)list[nameof(Criteria.SourceSigma)],
                    DisturberMean = (double)list[nameof(Criteria.DisturberMean)],
                    DisturberSigma = (double)list[nameof(Criteria.DisturberSigma)],
                    Threshold = (double)list[nameof(Criteria.Threshold)],
                    Name = key
                });
            }
        }
        private static void ReadProperties(XElement root, Dictionary<string, object> list) {
            foreach (XNode parameter in root.Nodes()) {
                list.Add(((XElement)parameter).Name.LocalName, ReadElement(root, ((XElement)parameter).Name.LocalName));
            }
        }
        private static object ReadElement(XElement root, string name) {
            XElement element = root.Element(name);
            if (element == null)
                return null;
            if (element.HasAttributes) {
                string value = element.Attribute(STR_Value).Value;
                string typeName = element.Attribute(STR_Type).Value;
                Type type = Type.GetType(typeName);
                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            Dictionary<string, object> list = new Dictionary<string, object>();
            ReadProperties(element, list);
            return list;
        }
        public static bool Save(Project project, string file) {
            XDocument doc = new XDocument();
            XElement root = new XElement("Project");
            XElement projectSettings = new XElement(STR_ProjectSettings);
            doc.Add(root);
            XElement criterias = new XElement(STR_Criterias);
            WriteElement(projectSettings, STR_CCThreshold, project.CCThreshold);
            root.Add(projectSettings);
            root.Add(criterias);
            foreach (Criteria criteria in project.Criterias) {
                XElement element = new XElement(criteria.Name);
                criterias.Add(element);
                Dictionary<string, object> list = new Dictionary<string, object>();
                list.Add(nameof(criteria.SourceMean), criteria.SourceMean);
                list.Add(nameof(criteria.SourceSigma), criteria.SourceSigma);
                list.Add(nameof(criteria.DisturberMean), criteria.DisturberMean);
                list.Add(nameof(criteria.DisturberSigma), criteria.DisturberSigma);
                list.Add(nameof(criteria.Threshold), criteria.Threshold);
                WriteProperties(element, list);
            }
            doc.Save(file);
            return true;
        }
        private static void WriteProperties(XElement root, Dictionary<string, object> list) {
            foreach (KeyValuePair<string, object> kvp in list) {
                WriteElement(root, kvp.Key, kvp.Value);
            }
        }

        private static void WriteElement(XElement root, string name, object value) {
            XElement element = new XElement(name);
            if (value is Dictionary<string, object>) {
                WriteProperties(element, (Dictionary<string, object>)value);
            } else if (value == null)
                return;
            else {
                element.SetAttributeValue(STR_Type, value.GetType());
                element.SetAttributeValue(STR_Value, Convert.ToString(value, CultureInfo.InvariantCulture));
            }
            root.Add(element);
        }        
    }
}
