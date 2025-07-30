using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace JsonToPdfClient
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Selecciona tu licencia (sin él sale la ventana de bienvenida)
            QuestPDF.Settings.License = LicenseType.Community;
            Application.Run(new LoginAuth());

            //// 1) Mostrar login
            //var loginForm = new LoginAuth();
            //loginForm.ShowDialog();

            //// 2) Si el login falló, salimos
            //if (!loginForm.Authenticated)
            //    return;

            //// 3) Arrancamos el formulario principal
            //Application.Run(new Form1());
        }
    }
}
