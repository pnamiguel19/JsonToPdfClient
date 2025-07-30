using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Diagnostics;
using static System.Net.WebRequestMethods;
using QuestPDF.Helpers;
using System.Linq;
using System.Net.Http.Headers;
using JsonToPdfClient.Services;



namespace JsonToPdfClient
{
    public partial class Form1 : Form
    {
        private readonly HttpClient _http = AuthService.HttpClient;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtUrl.Text = "https://localhost:44369/api/Reserva";
        }

        private async void btnFetch_Click(object sender, EventArgs e)
        {
            btnFetch.Enabled = false;
            try
            {
                var url = txtUrl.Text.Trim();
                string json = await _http.GetStringAsync(url);
                // Formatea con indentación
                var token = JToken.Parse(json);
                txtJson.Text = token.ToString(Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error al traer JSON",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnFetch.Enabled = true;
            }
        }

        private async void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            try
            {
                // 1) Leer URL y descargar JSON
                var endpoint = txtUrl.Text.Trim();
                string json = await _http.GetStringAsync(endpoint);

                // 2) Parsear a JArray
                var token = JToken.Parse(json);
                var items = token is JArray arr ? arr : new JArray(token);

                if (!items.Any())
                {
                    MessageBox.Show("El JSON está vacío.", "Atención",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 3) Columnas dinámicas
                var columns = items
                    .OfType<JObject>()
                    .SelectMany(o => o.Properties().Select(p => p.Name))
                    .Distinct()
                    .ToList();

                // 4) Título = último segmento de la URL
                var uri = new Uri(endpoint);
                var title = uri.Segments.Last().Trim('/');

                // 5) Diálogo de guardado
                using (var sfd = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = $"{title}.pdf"
                })
                {
                    if (sfd.ShowDialog() != DialogResult.OK)
                        return;

                    // 6) Generar PDF
                    Document.Create(doc =>
                    {
                        doc.Page(page =>
                        {
                            page.Size(PageSizes.A4);
                            page.Margin(20);
                            page.DefaultTextStyle(x => x.FontSize(9));

                            page.Header()
                                .Text(title)
                                .FontSize(16)
                                .SemiBold();

                            page.Content().PaddingVertical(10).Table(table =>
                            {
                                // Definir columnas
                                table.ColumnsDefinition(def =>
                                {
                                    foreach (var _ in columns)
                                        def.RelativeColumn();
                                });

                                // Encabezados
                                table.Header(header =>
                                {
                                    foreach (var colKey in columns)
                                        header.Cell()
                                              .Background("#EEEEEE")
                                              .Padding(4)
                                              .Text(colKey)
                                              .SemiBold();
                                });

                                // Filas
                                foreach (var item in items.OfType<JObject>())
                                {
                                    foreach (var colKey in columns)
                                    {
                                        var tokenValue = item[colKey];
                                        string text;

                                        // Si es valor primitivo, lo convertimos directamente
                                        if (tokenValue == null || tokenValue.Type == JTokenType.Null)
                                        {
                                            text = "";
                                        }
                                        else if (tokenValue.Type == JTokenType.Object)
                                        {
                                            var obj = (JObject)tokenValue;

                                            // Intentamos obtener la propiedad "Nombre" o "name"
                                            if (obj.TryGetValue("Nombre", StringComparison.OrdinalIgnoreCase, out var n1))
                                                text = n1.ToString();
                                            else if (obj.TryGetValue("name", StringComparison.OrdinalIgnoreCase, out var n2))
                                                text = n2.ToString();
                                            else
                                            {
                                                // Si no tiene "nombre", tomamos el primer campo que encontremos
                                                var firstProp = obj.Properties().FirstOrDefault();
                                                text = firstProp?.Value.ToString() ?? "";
                                            }
                                        }
                                        else if (tokenValue.Type == JTokenType.Array)
                                        {
                                            // Array: unimos sus elementos separados por coma
                                            text = string.Join(", ", tokenValue.Select(x => x.ToString()));
                                        }
                                        else
                                        {
                                            // Otros tipos (string, número, bool, fecha…)
                                            text = tokenValue.ToString();
                                        }

                                        table.Cell()
                                             .Padding(4)
                                             .Text(text);
                                    }
                                }
                            });

                            page.Footer()
                                .AlignCenter()
                                .Text(f =>
                                {
                                    f.Span("Página ");
                                    f.CurrentPageNumber();
                                    f.Span(" de ");
                                    f.TotalPages();
                                });
                        });
                    })
                    .GeneratePdf(sfd.FileName);

                    Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error al imprimir PDF",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnPrint.Enabled = true;
            }
        }
    }
}
