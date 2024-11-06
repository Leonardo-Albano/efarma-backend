using EFarma.Models;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace EFarma.Utils
{
    public class MailManager
    {
        public static async Task NotifyPendentPrescriptions(Employee employee, StockRoom stockRoom, List<Prescription> prescriptions)
        {
            var messageBuilder = new StringBuilder();

            messageBuilder.AppendLine($"Uma retirada indevida foi feita pelo seguinte funcionário:");

            messageBuilder = BuildEmployeeBody(employee, messageBuilder);
            messageBuilder = BuildStockRoomBody(stockRoom, messageBuilder);
            messageBuilder = BuildPrescriptionsBody(prescriptions, messageBuilder);

            string mailTitle = $"Saída da sala de estoque com receitas pendentes: {employee.Name}";

            await SendMail(mailTitle, messageBuilder.ToString(), employee.ResponsibleMail);
        }

        public static async Task NotifyMedicamentsTaken(Employee employee, StockRoom stockRoom, List<InStockItem> medicamentHasBeenTaken)
        {
            var messageBuilder = new StringBuilder();

            messageBuilder.AppendLine($"Uma retirada indevida foi feita pelo seguinte funcionário:");
            messageBuilder = BuildEmployeeBody(employee, messageBuilder);
            messageBuilder = BuildStockRoomBody(stockRoom, messageBuilder);
            messageBuilder = BuildMedicamentsHasBeenTakenBody(medicamentHasBeenTaken, messageBuilder);

            string mailTitle = $"Saída da sala de estoque com medicamentos retirados indevidamente: {employee.Name}";

            await SendMail(mailTitle, messageBuilder.ToString(), employee.ResponsibleMail);
        }

        private static async Task SendMail(string title, string body, string receiverMail)
        {
            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress("MS_xoSR4R@trial-pq3enl6w73842vwr.mlsender.net");
                mail.To.Add(receiverMail);
                mail.Subject = title;
                mail.Body = body;

                using (var smtp = new SmtpClient("smtp.mailersend.net")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("MS_xoSR4R@trial-pq3enl6w73842vwr.mlsender.net", "fQkKwLxM1OXZnVm9"),
                    EnableSsl = true,
                })
                {
                    int retries = 3;
                    while (retries > 0)
                    {
                        try
                        {
                            await smtp.SendMailAsync(mail);
                            break;
                        }
                        catch (SmtpException ex)
                        {
                            retries--;
                            if (retries == 0)
                            {
                                Console.WriteLine($"Failed to send email: {ex.Message}");
                                throw;
                            }
                        }
                    }
                }
            }
        }

        private static StringBuilder BuildEmployeeBody(Employee employee, StringBuilder messageBuilder)
        {
            messageBuilder.AppendLine("==============================================================");

            messageBuilder.AppendLine($"Nome: {employee.Name}");
            messageBuilder.AppendLine($"Email: {employee.Mail}");
            messageBuilder.AppendLine($"Telefone: {employee.Phone}");

            if (!string.IsNullOrEmpty(employee.EmployeeId))
            {
                messageBuilder.AppendLine($"Id de funcionário: {employee.EmployeeId}");
            }
            messageBuilder.AppendLine("==============================================================");

            return messageBuilder;
        }

        private static StringBuilder BuildStockRoomBody(StockRoom stockRoom, StringBuilder messageBuilder)
        {
            messageBuilder.AppendLine("==============================================================");
            messageBuilder.AppendLine($"Sala de estoque:");
            messageBuilder.AppendLine($"Nome: {stockRoom.Name}");
            messageBuilder.AppendLine($"Endereço: {stockRoom.Address}");
            messageBuilder.AppendLine("==============================================================");

            return messageBuilder;
        }

        private static StringBuilder BuildPrescriptionsBody(List<Prescription> prescriptions, StringBuilder messageBuilder)
        {
            messageBuilder.AppendLine("==============================================================");
            messageBuilder.AppendLine("Receita(s):");

            foreach (var prescription in prescriptions)
            {
                messageBuilder.AppendLine("--------------------------------------------------------------");

                messageBuilder.AppendLine($"Id: {prescription.Id}");
                messageBuilder.AppendLine($"Data de Criação: {prescription.Date}");
                messageBuilder.AppendLine("Itens:");

                foreach (var item in prescription.Items)
                {
                    string medicamentName = $"{item.Medicament.Description} {item.Medicament.Dosage}{item.Medicament.Measure}";
                    messageBuilder.AppendLine($"   {medicamentName}");
                }
            }
            messageBuilder.AppendLine("--------------------------------------------------------------\n");

            messageBuilder.AppendLine("==============================================================");

            return messageBuilder;
        }

        private static StringBuilder BuildMedicamentsHasBeenTakenBody(List<InStockItem> medicamentHasBeenTaken, StringBuilder messageBuilder)
        {
            messageBuilder.AppendLine("==============================================================");
            messageBuilder.AppendLine("Lista do(s) medicamento(s) retirado(s) indevidamente:");
           
            foreach(var item in medicamentHasBeenTaken)
            {
                messageBuilder.AppendLine("--------------------------------------------------------------");
                messageBuilder.AppendLine($"Nome do medicamento: {item.Medicament.GetMedicamentName()}");
                messageBuilder.AppendLine($"Código do medicamento: {item.TagCode}");
            }
            messageBuilder.AppendLine("--------------------------------------------------------------\n");


            messageBuilder.AppendLine("==============================================================");

            return messageBuilder;
        }
    }
}
