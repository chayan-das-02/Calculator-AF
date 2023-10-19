using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web.Http;
using Calculator.Models;

namespace Calculator
{
    public static class Calculator
    {
        [FunctionName("Calculator")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject<ArithmeticOperation>(requestBody);
                /*string number1 = data?.number1;
                string number2 = data?.number2;
                string sign = data?.sign;*/

                string responseMessage = null;

                if (Valid_Number(data.number1, data.number2))
                {
                    responseMessage = Calc(data.number1, data.number2, data.sign);
                    return new OkObjectResult("The result is: " + responseMessage);
                }
                else
                {
                    return new BadRequestObjectResult("Exception caught: Invalid data");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new BadRequestObjectResult("Exception caught: " + ex.Message);
            }
        }

        public static bool Valid_Number(string number1, string number2)
        {
            if (int.TryParse(number1, out int num1) && int.TryParse(number2, out int num2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string Calc(string number1, string number2, string sign)
        {
            string responseMessage = null;
            int result;
            switch (sign)
            {
                case "+":
                    result = int.Parse(number1) + int.Parse(number2);
                    responseMessage = result.ToString();
                    break;
                case "-":
                    result = int.Parse(number1) - int.Parse(number2);
                    responseMessage = result.ToString();
                    break;
                case "*":
                    result = int.Parse(number1) * int.Parse(number2);
                    responseMessage = result.ToString();
                    break;
                case "/":
                    if (number2 == "0")
                    {
                        throw new DivideByZeroException("Attempted to divide by zero.");
                    }
                    result = int.Parse(number1) / int.Parse(number2);
                    responseMessage = result.ToString();
                    break;
                default:
                    throw new ArgumentException("Invalid operator");
            }
            return responseMessage;
        }
    }
}
