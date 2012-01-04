using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Jeton
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        readonly Java2CsSyntaxConverter m_converter = new Java2CsSyntaxConverter();

        private const string DebugSrcFolder = @"C:\jctest\java\de\danielnaber\languagetool";
        //private string m_debugSrcFolder = @"C:\jctest\java";
        private const string DebugDstFolder = @"D:\SVNRep\Projects\SemanticChecker\trunk\Incubator\RuleBasedLanguageTool\SharpLanguageTool";
        //private string m_debugDstFolder = @"C:\jctest\cs";

        private void BtnConvertClick(object sender, EventArgs e)
        {
            rtbOutput.Text = "";
            rtbOutput.Text = m_converter.Convert(rtbInput.Text);
        }


        private void ShowStatus(string status)
        {
            this.Invoke(new Action(() => statusLabel.Text = status));
        }

        private void ToggleEnable(bool enable)
        {
            this.Invoke(new Action(() => 
            {
                btnBulkConvert.Text = enable ? "Bulk Convert" : "Stop";
            }));
        }

        Thread m_threadConverter = null;

        private void BtnBulkConvertClick(object sender, EventArgs e)
        {
            if (btnBulkConvert.Text == "Stop")
            {
                if (m_threadConverter != null && m_threadConverter.IsAlive)
                    m_threadConverter.Abort();

                ToggleEnable(true);
                return;
            }

            try
            {
                string dstFolder;
                string srcFolder;

                ShowStatus("Select Source Folder");

                using (var dlg = new FolderBrowserDialog())
                {
                    if (!String.IsNullOrEmpty(DebugSrcFolder))
                        dlg.SelectedPath = DebugSrcFolder;

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        srcFolder = dlg.SelectedPath;
                    }
                    else
                    {
                        return;
                    }
                }

                ShowStatus("Select Destination Folder");

                using (var dlg = new FolderBrowserDialog())
                {
                    if (!String.IsNullOrEmpty(DebugDstFolder))
                        dlg.SelectedPath = DebugDstFolder;

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        dstFolder = dlg.SelectedPath;
                    }
                    else
                    {
                        return;
                    }
                }

                srcFolder = Path.GetFullPath(srcFolder);
                dstFolder = Path.GetFullPath(dstFolder);

                m_threadConverter = new Thread(() =>
                    {
                        ToggleEnable(false);

                        foreach (string file in GetFilesRecursively(srcFolder, "*.java"))
                        {
                            string absPath = Path.GetFullPath(file);
                            ShowStatus(String.Format("Reading {0}", absPath));

                            string remPath = absPath.Substring(srcFolder.Length);

                            // remove leading separators if any
                            if (remPath.StartsWith("/") || remPath.StartsWith("\\"))
                            {
                                remPath = remPath.Substring(1);
                            }

                            string dstPath = Path.Combine(dstFolder, remPath);
                            dstPath = Path.ChangeExtension(dstPath, "cs");

                            string srcJava = File.ReadAllText(absPath);
                            string resCs = m_converter.Convert(srcJava);

                            ShowStatus(String.Format("Writing {0}", dstPath));

                            string dstDir = Path.GetDirectoryName(dstPath);
                            Debug.Assert(dstDir != null, "dstDir != null");
                            if (!Directory.Exists( dstDir))
                            {
                                Directory.CreateDirectory(dstDir);
                            }

                            File.WriteAllText(dstPath, resCs);
                        }
                        ShowStatus("Ready");
                        ToggleEnable(true);
                    });

                m_threadConverter.Start();

            }
            finally
            {
                ShowStatus("Ready");
            }


        }

        private string[] GetFilesRecursively(string folder, string searchPattern)
        {
            return Directory.GetFiles(folder, searchPattern, SearchOption.AllDirectories);
        }

    }
}
