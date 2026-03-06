using System.Text;
using System.Xml;
using CloudFileServer.Domain.Interfaces;
using CloudFileServer.Domain.Models.TreeItems;

namespace CloudFileServer.Services.Visitors;

public class XmlSerializationVisitor : INodeVisitor
{
    private readonly StringBuilder _sb = new();
    private readonly XmlWriter _writer;

    public XmlSerializationVisitor()
    {
        _writer = XmlWriter.Create(_sb, new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "    ",
            OmitXmlDeclaration = true
        });
    }

    public string GetXml()
    {
        _writer.Close();
        return _sb.ToString();
    }

    public void EnterDirectory(DirectoryTreeItem directory)
    {
        _writer.WriteStartElement(SanitizeTagName(directory.Name));
    }

    public void LeaveDirectory(DirectoryTreeItem directory)
    {
        _writer.WriteEndElement();
    }

    public void Visit(ImageFileTreeItem image)
    {
        _writer.WriteStartElement(SanitizeTagName(image.Name, image.StoragePath));
        _writer.WriteString($"解析度: {image.WidthPx}×{image.HeightPx}, 大小: {FormatBytes(image.SizeBytes)}");
        _writer.WriteEndElement();
    }

    public void Visit(TextFileTreeItem text)
    {
        _writer.WriteStartElement(SanitizeTagName(text.Name, text.StoragePath));
        _writer.WriteString($"編碼: {text.Encoding}, 大小: {FormatBytes(text.SizeBytes)}");
        _writer.WriteEndElement();
    }

    public void Visit(WordFileTreeItem word)
    {
        _writer.WriteStartElement(SanitizeTagName(word.Name, word.StoragePath));
        _writer.WriteString($"頁數: {word.PageCount}, 大小: {FormatBytes(word.SizeBytes)}");
        _writer.WriteEndElement();
    }

    private static string SanitizeTagName(string name, string? storagePath = null)
    {
        var tagName = name.Replace(' ', '_').Replace('-', '_');

        if (storagePath is not null)
        {
            var dotIndex = storagePath.LastIndexOf('.');
            if (dotIndex >= 0)
                tagName += "_" + storagePath.Substring(dotIndex + 1);
        }

        if (tagName.Length > 0 && (char.IsDigit(tagName[0]) || tagName[0] == '-'))
            tagName = "_" + tagName;

        return tagName;
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes < 1024) return $"{bytes}B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024}KB";
        return $"{bytes / (1024 * 1024)}MB";
    }
}
