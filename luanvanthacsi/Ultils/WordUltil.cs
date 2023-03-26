using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace luanvanthacsi.Ultils
{
    public class WordUltil
    {
        public static string DocxTemplateFolder { get; set; }
        private static XNamespace w =
           "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        public static byte[] WriteDOCX<T>(string fileName, T data)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("vi-VN");
                string directory = DateTime.Now.ToString("yyyyMMdd");
                string fileTemplate = System.IO.Path.Combine(DocxTemplateFolder, fileName);
                byte[] result = new byte[0];

                using (WordprocessingDocument doc = WordprocessingDocument.Open(fileTemplate, false))
                {
                    string dataStr = GenerateXML(data);
                    result = GenerateDocuments(dataStr, "./Content", fileTemplate);
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static string GenerateXML(object o)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;
            string utf8 = "";
            try
            {
                XmlSerializer serializer = new XmlSerializer(o.GetType());
                using (StringWriter writer = new Utf8StringWriter())
                {
                    serializer.Serialize(writer, o);
                    utf8 = writer.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }
            }
            return utf8;
        }
        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }


        private static byte[] GenerateDocuments(string dataStr, string selectDocuments, string templateFile)
        {
            try
            {
                byte[] result = new byte[0];
                var dataFile = XElement.Parse(dataStr);
                var documents = dataFile.XPathSelectElements(selectDocuments);
                foreach (var document in documents)
                {
                    result = GenerateDocument(document, templateFile);
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        static byte[] GenerateDocument(XElement document, string templateFile)
        {
            try
            {
                byte[] byteArray = File.ReadAllBytes(templateFile);
                using (MemoryStream mem = new MemoryStream())
                {
                    mem.Write(byteArray, 0, byteArray.Length);
                    using (WordprocessingDocument wordDoc =
                        WordprocessingDocument.Open(mem, true))
                    {
                        XDocument xDoc = wordDoc.MainDocumentPart.GetXDocument();
                        XElement newRootElement = (XElement)Transform(
                            wordDoc.MainDocumentPart.GetXDocument().Root, document, wordDoc);
                        xDoc.Elements().First().ReplaceWith(newRootElement);
                        wordDoc.MainDocumentPart.PutXDocument();

                    }
                    return mem.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        static object Transform(XNode node, XElement document, WordprocessingDocument wordDoc)
        {
            XElement element = node as XElement;
            if (element != null)
            {
                if (element.Name == w + "sdt")
                {
                    string tag = element.Elements(w + "sdtPr")
                        .Elements(w + "alias")
                        .Attributes(w + "val")
                        .FirstOrDefault()
                        .Value;
                    if (tag == "Config") return null;
                    if (tag == "SelectValue")
                    {
                        XElement run = element.Element(w + "sdtContent").Element(w + "r");
                        string valueSelector = GetContentControlContents(element);
                        var selsectedElement = document.XPathSelectElement(valueSelector);
                        if (selsectedElement != null)
                        {
                            string newValue = selsectedElement.Value;
                            if (newValue.Contains("`"))
                            {
                                List<XElement> elements = new List<XElement>();
                                var values = newValue.Split('`');
                                var elementBreak = new XElement(w + "br");
                                var elementTab = new XElement(w + "tab");
                                var lastItem = values.Last();
                                foreach (var item in values)
                                {

                                    if (item.Contains("^"))
                                    {
                                        var valueTabs = item.Split('^');
                                        var elementTabs = valueTabs.Where(c => c != null).SelectMany(c => new List<XElement> { new XElement(w + "t", c), elementBreak, elementTab }).ToList();
                                        elements.AddRange(elementTabs);
                                    }
                                    else
                                    {
                                        elements.Add(new XElement(w + "t", item));
                                        if (item != lastItem)
                                        {
                                            elements.Add(elementBreak);
                                        }
                                    }
                                }

                                var ele = new XElement(w + "r",
                                    run.Elements().Where(e => e.Name != w + "t"),
                                    elements);
                                return ele;
                            }
                            else if (newValue.Contains("^"))
                            {
                                var values = newValue.Split('^');
                                var elements = values.SelectMany(c => new List<XElement> { new XElement(w + "t", c), new XElement(w + "br"), new XElement(w + "tab") }).ToList();
                                elements.RemoveAt(elements.Count() - 1);
                                var ele = new XElement(w + "r",
                                    run.Elements().Where(e => e.Name != w + "t"),
                                    elements);
                                return ele;
                            }
                            else
                            {
                                var ele = new XElement(w + "r",
                                    run.Elements().Where(e => e.Name != w + "t"),
                                    new XElement(w + "t", newValue));
                                return ele;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    if (tag == "CheckValue")
                    {
                        XElement run = element.Element(w + "sdtContent").Element(w + "r");
                        string valueSelector = GetContentControlContents(element);
                        var selsectedElement = document.XPathSelectElement(valueSelector);
                        if (selsectedElement != null)
                        {
                            string newValue = selsectedElement.Value;
                            string checkedBox = "00FE";
                            string checkedBoxX = "0054";
                            string uncheckedBox = "00A8";
                            string checkedBoxXCambria = "22A0";
                            if (newValue == "1" || newValue == "true")
                            {
                                var ele = new XElement(w + "r", run.Elements().Where(e => e.Name != w + "t"));
                                var symNode = new XElement(w + "sym");
                                symNode.SetAttributeValue(w + "font", "Wingdings");
                                symNode.SetAttributeValue(w + "char", checkedBox);
                                ele.Add(symNode);
                                return ele;
                            }
                            else if (newValue == "x")
                            {
                                var ele = new XElement(w + "r", run.Elements().Where(e => e.Name != w + "t"));
                                var symNode = new XElement(w + "sym");
                                symNode.SetAttributeValue(w + "font", "Cambria");
                                symNode.SetAttributeValue(w + "char", checkedBoxXCambria);
                                ele.Add(symNode);
                                return ele;
                            }
                            else
                            {
                                var ele = new XElement(w + "r", run.Elements().Where(e => e.Name != w + "t"));
                                var symNode = new XElement(w + "sym");
                                symNode.SetAttributeValue(w + "font", "Wingdings");
                                symNode.SetAttributeValue(w + "char", uncheckedBox);
                                ele.Add(symNode);
                                return ele;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    if (tag == "Repeat")
                    {
                        XElement contentContentControl = GetContentControlByTag(element, "Content");
                        XElement selectRepeatingDataContentControl =
                            GetContentControlByTag(element, "SelectRepeatingData");
                        string selector = GetContentControlContents(selectRepeatingDataContentControl);
                        var repeatingData = document.XPathSelectElements(selector);
                        var newContent = repeatingData.Select(d =>
                        {
                            var content = contentContentControl.Element(w + "sdtContent")
                                .Elements().Select(e => Transform(e, d, wordDoc)).ToList();
                            return content;
                        })
                            .ToList();
                        return newContent;
                    }
                    if (tag == "Table")
                    {
                        XElement content = GetContentControlByTag(element, "Content");
                        XElement selectRowsContentControl =
                            GetContentControlByTag(element, "SelectRows");
                        string selector = GetContentControlContents(selectRowsContentControl);
                        var tableData = document.XPathSelectElements(selector);
                        XElement table = content.Descendants(w + "tbl").FirstOrDefault();
                        XElement protoRow = table.Elements(w + "tr").Skip(2).FirstOrDefault();
                        XElement newTable = new XElement(w + "tbl",
                            table.Elements().Where(e => e.Name != w + "tr"),
                            table.Elements(w + "tr").FirstOrDefault(),
                            table.Elements(w + "tr").Skip(1).FirstOrDefault(),
                            tableData.Select(d =>
                                new XElement(w + "tr",
                                    protoRow.Elements().Where(r => r.Name != w + "tc"),
                                    protoRow.Elements(w + "tc")
                                        .Select(tc =>
                                        {
                                            XElement paragraph = tc.Elements(w + "p")
                                                .FirstOrDefault();
                                            XElement run = paragraph.Elements(w + "r")
                                                .FirstOrDefault();
                                            string cellSelector = paragraph.Value;
                                            if (!cellSelector.StartsWith("."))
                                            {
                                                return null;
                                            }
                                            var selectedElement = d.XPathSelectElement(cellSelector);
                                            string cellData;
                                            if (selectedElement != null)
                                            {
                                                cellData = selectedElement.Value;
                                            }
                                            else
                                            {
                                                cellData = "";
                                            }
                                            XElement newCell = new XElement(w + "tc",
                                                tc.Elements().Where(z => z.Name != w + "p"),
                                                new XElement(w + "p",
                                                    paragraph.Elements().Where(z1 => z1.Name != w + "r"),
                                                    new XElement(w + "r",
                                                        run.Elements().Where(z2 => z2.Name != w + "t"),
                                                        new XElement(w + "t", cellData))));
                                            return newCell;
                                        }))));
                        return newTable;
                    }
                    if (tag == "Table2")
                    {
                        XElement content = GetContentControlByTag(element, "Content");
                        XElement selectRowsContentControl =
                            GetContentControlByTag(element, "SelectRows");
                        string selector = GetContentControlContents(selectRowsContentControl);
                        var tableData = document.XPathSelectElements(selector);
                        XElement table = content.Descendants(w + "tbl").FirstOrDefault();
                        XElement protoRow = table.Elements(w + "tr").LastOrDefault();
                        XElement newTable = new XElement(w + "tbl",
                            table.Elements().Where(e => e.Name != w + "tr"),
                            table.Elements(w + "tr").FirstOrDefault(),
                            tableData.Select(d =>
                                new XElement(w + "tr",
                                    protoRow.Elements().Where(r => r.Name != w + "tc"),
                                    protoRow.Elements(w + "tc")
                                        .Select(tc =>
                                        {
                                            XElement paragraph = tc.Elements(w + "p")
                                                .FirstOrDefault();
                                            XElement run = paragraph.Elements(w + "r")
                                                .FirstOrDefault();
                                            string cellSelector = paragraph.Value;
                                            if (!cellSelector.StartsWith("."))
                                            {
                                                return null;
                                            }
                                            var selectedElement = d.XPathSelectElement(cellSelector);
                                            string cellData;
                                            if (selectedElement != null)
                                            {
                                                cellData = selectedElement.Value;
                                            }
                                            else
                                            {
                                                cellData = "";
                                            }
                                            XElement newCell = new XElement(w + "tc",
                                                tc.Elements().Where(z => z.Name != w + "p"),
                                                new XElement(w + "p",
                                                    paragraph.Elements().Where(z1 => z1.Name != w + "r"),
                                                    new XElement(w + "r",
                                                        run.Elements().Where(z2 => z2.Name != w + "t"),
                                                        new XElement(w + "t", cellData))));
                                            return newCell;
                                        }))));
                        return newTable;
                    }
                    if (tag == "Table3")
                    {
                        XElement content = GetContentControlByTag(element, "Content");
                        XElement selectRowsContentControl =
                            GetContentControlByTag(element, "SelectRows");
                        string selector = GetContentControlContents(selectRowsContentControl);
                        var tableData = document.XPathSelectElements(selector);
                        XElement table = content.Descendants(w + "tbl").FirstOrDefault();
                        XElement protoRow = table.Elements(w + "tr").LastOrDefault();
                        XElement newTable = new XElement(w + "tbl",
                            table.Elements().Where(e => e.Name != w + "tr"),
                            table.Elements(w + "tr").FirstOrDefault(),
                            tableData.Select(d =>
                                new XElement(w + "tr",
                                    protoRow.Elements().Where(r => r.Name != w + "tc"),
                                    protoRow.Elements(w + "tc")
                                        .Select(tc =>
                                        {
                                            var content = tc.Elements().Select(e => Transform(e, d, wordDoc)).ToList();
                                            XElement newCell = new XElement(w + "tc",
                                                tc.Elements().Where(z => z.Name != w + "p"),
                                                content);
                                            return newCell;
                                        }))));
                        return newTable;
                    }
                    
                }
                return new XElement(element.Name,
                    element.Attributes(),
                    element.Nodes().Select(n => Transform(n, document, wordDoc)));
            }
            return node;
        }

        static XElement GetContentControlByTag(XContainer ancestor, string tag)
        {
            return ancestor.Descendants(w + "sdt")
                .Where(e => e.Elements(w + "sdtPr")
                    .Elements(w + "alias")
                    .Attributes(w + "val")
                    .FirstOrDefault().Value == tag)
                .FirstOrDefault();
        }

        static string GetContentControlContents(XElement contentControl)
        {
            return contentControl.Element(w + "sdtContent")
                .Descendants(w + "t")
                .Select(t => (string)t)
                .StringConcatenate();
        }
       
    }

    public static class LocalExtensions
    {
        public static string StringConcatenate(this IEnumerable<string> source)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string item in source) sb.Append(item);
            return sb.ToString();
        }
        public static XDocument GetXDocument(this OpenXmlPart part)
        {
            XDocument xdoc = part.Annotation<XDocument>();
            if (xdoc != null) return xdoc;
            using (StreamReader sr = new StreamReader(part.GetStream()))
            using (XmlReader xr = XmlReader.Create(sr))
                xdoc = XDocument.Load(xr);
            part.AddAnnotation(xdoc);
            return xdoc;
        }
        public static void PutXDocument(this OpenXmlPart part)
        {
            XDocument xdoc = part.GetXDocument();
            if (xdoc != null)
                using (XmlWriter xw = XmlWriter.Create(part.GetStream
                  (FileMode.Create, FileAccess.Write)))
                    xdoc.Save(xw);
        }
    }

}
