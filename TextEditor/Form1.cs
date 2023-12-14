using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TextEditor
{
    public partial class Form1 : Form
    {
        private string autosaveFileName = "autosave.txt";
        Encoding currentEncoding = Encoding.UTF8;
        Encoding selectedEncoding = Encoding.UTF8;
        Encoding windows1251Encoding = Encoding.GetEncoding("Windows-1251");
        private List<Document> documents = new List<Document>();
        private Document currentDocument;

        public class Document
        {
            public string content { get; set; }
            public string filePath { get; set; }

            public override string ToString()
            {
                return Path.GetFileName(filePath);
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Interval = 300000;
            timer1.Start();
            string autosaveFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, autosaveFileName);
            if (File.Exists(autosaveFilePath))
            {
                richTextBox1.Text = File.ReadAllText(autosaveFilePath);
            }
        }

        private void fontDialog1_Apply(object sender, EventArgs e)
        {

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentDocument = new Document();
            richTextBox1.Clear();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select file";
            openFileDialog1.Multiselect = false;
            openFileDialog1.Filter = "Text Files(.txt)|*.txt|All Files (.)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Document newDocument = new Document();
                newDocument.filePath = openFileDialog1.FileName;
                using (StreamReader streamReader = new StreamReader(openFileDialog1.FileName, selectedEncoding))
                {
                    newDocument.content = streamReader.ReadToEnd();
                }
                documents.Add(newDocument);
                currentDocument = newDocument;
                richTextBox1.Text = currentDocument.content;

                listBox1.Items.Add(newDocument);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentDocument != null)
            {
                SaveDocument(currentDocument);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Save document";
            saveFileDialog1.Filter = "Text Files(.txt)|*.txt|All Files (.)|*.*";
            saveFileDialog1.DefaultExt = "txt";

            Document docToSave = currentDocument ?? new Document();

            SaveDocument(docToSave);
            currentDocument = docToSave;
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
        }

        private void colorThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                colorDialog1.ShowDialog();
                richTextBox1.SelectionColor = colorDialog1.Color;
            }
        }

        private void fontStyleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                fontDialog1.ShowDialog();
                richTextBox1.SelectionFont = fontDialog1.Font;
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            SaveTextToFile(autosaveFileName);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SaveTextToFile(autosaveFileName);
        }

        private void SaveTextToFile(string fileName)
        {
            string textToSave = richTextBox1.Text;
            string autosaveFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            File.WriteAllText(autosaveFilePath, textToSave);
        }

        private void changeFileEncodingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedEncoding == Encoding.UTF8)
            {
                selectedEncoding = windows1251Encoding;
            }
            else
            {
                selectedEncoding = Encoding.UTF8;
            }
            changeFileEncodingToolStripMenuItem.Text = selectedEncoding == Encoding.UTF8 ? "Switch to Windows-1251" : "Switch to UTF-8";
        }

        private void SaveDocument(Document doc)
        {
            if (doc != null)
            {
                if (string.IsNullOrEmpty(doc.filePath) || Path.GetExtension(doc.filePath) != ".txt")
                {
                    saveFileDialog1.Filter = "Text Files(.txt)|*.txt|All Files (.)|*.*";

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        doc.filePath = saveFileDialog1.FileName;
                    }
                    else
                    {
                        return;
                    }
                }

                using (StreamWriter streamWriter = new StreamWriter(doc.filePath, false, selectedEncoding))
                {
                    streamWriter.Write(richTextBox1.Text);
                }
                doc.content = richTextBox1.Text;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                currentDocument = (Document)listBox1.SelectedItem;
                richTextBox1.Text = currentDocument.content;
            }
        }
    }
}