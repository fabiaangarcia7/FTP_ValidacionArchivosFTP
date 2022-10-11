using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Globalization;
using System.IO;
using WinSCP;

namespace FTP_ValidacionArchivosFTP
{

    public partial class Form1 : Form
    {
        SessionOptions sessionOp;
        public Form1()
        {

            sessionOp = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                HostName = "127.0.0.1",
                UserName = "admin",
                Password = "qwerty123"
            };

            InitializeComponent();


        }

        private void btnOpenFileDialog_Click(object sender, EventArgs e)
        {

            try
            {
                opnFileDialog.Title = "Selecciona la ruta";
                opnFileDialog.ShowDialog();
                string Texto = opnFileDialog.FileName;

                txtRuta.Text = Texto;
            }
            catch(Exception)
            {
                MessageBox.Show("Error al cargar la ruta");
            }
            
        }

        private void btnValidar_Click(object sender, EventArgs e)
        {
            try
            {
                using (Session session = new Session())
                {

                    // Connect
                    session.Open(sessionOp);

                    string stamp = DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                    string fileName = "Prueba" + stamp + ".txt";
                    string remotePath = "/FTP/" + fileName;
                    string localPath = @"C:\Users\fabian.garcia.SA\Documents\Subir Archivos\" + fileName;

                    // Manual "remote to local" synchronization.

                    // You can achieve the same using:
                    // session.SynchronizeDirectories(
                    //     SynchronizationMode.Local, localPath, remotePath, false, false,
                    //     SynchronizationCriteria.Time,
                    //     new TransferOptions { FileMask = fileName }).Check();
                    if (session.FileExists(remotePath))
                    {
                        bool download;
                        if (!File.Exists(localPath))
                        {
                            Console.WriteLine(
                                "File {0} exists, local backup {1} does not",
                                remotePath, localPath);
                            download = true;
                        }
                        else
                        {
                            DateTime remoteWriteTime =
                                session.GetFileInfo(remotePath).LastWriteTime;
                            DateTime localWriteTime = File.GetLastWriteTime(localPath);

                            if (remoteWriteTime > localWriteTime)
                            {
                                Console.WriteLine(
                                    "File {0} as well as local backup {1} exist, " +
                                    "but remote file is newer ({2}) than local backup ({3})",
                                    remotePath, localPath, remoteWriteTime, localWriteTime);
                                download = true;
                            }
                            else
                            {
                                Console.WriteLine(
                                    "File {0} as well as local backup {1} exist, " +
                                    "but remote file is not newer ({2}) than local backup ({3})",
                                    remotePath, localPath, remoteWriteTime, localWriteTime);
                                download = false;
                            }
                        }

                        if (download)
                        {
                            // Download the file and throw on any error
                            session.GetFiles(remotePath, localPath).Check();

                            Console.WriteLine("Download to backup done.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("File {0} does not exist yet", remotePath);
                    }
                }
               
            }
            catch(Exception)
            {
                MessageBox.Show("Ha ocurrido un error al validar los archivos.");
            }

          }
    }
}