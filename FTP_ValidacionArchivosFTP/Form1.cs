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
                    string fileName = txtArchivo.Text;
                    fileName = "Prueba.txt";
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
                            MessageBox.Show("No existe el archivo en la direccion local. Se descargará el archivo desde el servidor");
                            download = true;
                        }
                        else
                        {
                            DateTime remoteWriteTime =
                                session.GetFileInfo(remotePath).LastWriteTime;
                            DateTime localWriteTime = File.GetLastWriteTime(localPath);

                            if (remoteWriteTime > localWriteTime)
                            {
                                MessageBox.Show("El archivo" + remotePath.ToString() + " existe en el directorio local, " +
                                    "pero la versión del servidor es más reciente " + remoteWriteTime.ToString() + ". Se descargara el archivo desde el servidor");
                                download = true;
                            }
                            else
                            {

                                MessageBox.Show("El archivo" + remotePath.ToString() + " existe en el directorio local, " +
                                   "pero la versión del servidor es más antigua " + remoteWriteTime.ToString() + ".");
                                download = false;
                            }
                        }

                        if (download)
                        {
                            // Download the file and throw on any error
                            session.GetFiles(remotePath, localPath).Check();

                            MessageBox.Show("Se ha descargado el archivo en la siguiente ubicación " +localPath.ToString()+".");
                        }
                    }
                    else
                    {
                        MessageBox.Show("El archivo no existe en la ubicación remota " + remotePath.ToString() + ".");
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