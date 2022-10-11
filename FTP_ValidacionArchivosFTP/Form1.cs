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

                    // Upload files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    TransferOperationResult transferResult;
                    transferResult =
                    session.PutFiles(@"C:\Users\fabian.garcia.SA\Documents\Subir Archivos\*", "/FTP/", false, transferOptions);

                    // Throw on any error
                    transferResult.Check();

                    // Print results
                    foreach (TransferEventArgs transfer in transferResult.Transfers)
                    {
                        Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                    }

                    DateTime remoteWriteTime = session.GetFileInfo("/FTP/Prueba.txt").LastWriteTime;
                    DateTime localWriteTime = File.GetLastWriteTime(@"C:\Users\fabian.garcia.SA\Documents\Subir Archivos\Prueba.txt");

                }
            }
            catch(Exception)
            {
                MessageBox.Show("Ha ocurrido un error al validar los archivos.");
            }

          }
    }
}