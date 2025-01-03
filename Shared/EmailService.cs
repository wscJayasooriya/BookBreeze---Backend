using System.Net.Mail;
using System.Net;
using E_Book.Dto;

namespace E_Book.Shared
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _fromEmail = "schinthaka63@gmail.com";
        private readonly string _fromEmailPassword = "lfjg oumk fwlf ppdh";

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient(_smtpServer)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_fromEmail, _fromEmailPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public string GenerateOrderEmailBody(OrderDto orderDto, object responseData)
        {
            var orderItemsHtml = string.Join("", orderDto.OrderItems.Select(item =>
                $"<tr><td>{item.BookName}</td><td>{item.Qty}</td><td>LKR {item.Price:F2}</td><td>LKR {item.TotalPrice:F2}</td></tr>"));

            var paymentDetailsHtml = string.Join("", orderDto.OrderPayments.Select(payment =>
                $"<tr><td>{payment.PaymentType}</td><td>LKR {payment.Amount:F2}</td><td>{payment.PaymentDate}</td></tr>"));

            return $@"
        <h3>Order Confirmation</h3>
        <p>Dear {orderDto.CustomerName},</p>
        <p>Thank you for your order! Below are the details of your order:</p>
        <h4>{responseData} </h4>
        <table border='1' style='border-collapse: collapse; width: 75%;'>
            <thead>
                <tr><th>Book Name</th><th>Quantity</th><th>Price</th><th>Subtotal</th></tr>
            </thead>
            <tbody>
                {orderItemsHtml}
            </tbody>
        </table>
        <h4>Payment Details:</h4>
        <table border='1' style='border-collapse: collapse; width: 50%;'>
            <thead>
                <tr><th>Payment Type</th><th>Amount</th><th>Payment Date</th></tr>
            </thead>
            <tbody>
                {paymentDetailsHtml}
            </tbody>
        </table>
        <p>If you have any questions, feel free to contact us.</p>
        <p>Best regards,</p>
        <p>BookBreeze Team</p>";
        }
    }
}
