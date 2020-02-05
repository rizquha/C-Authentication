using System.IO;

namespace RestAPI
{
    public class Token
    {
        public string token{get;set;}
        public string GetSavedToken()
        {
            return System.IO.File.ReadAllText("token.txt");
        }
        public bool SaveToken()
        {
            if(token != null)
            {
                var fToken = new StreamWriter("token.txt");
                fToken.WriteLine(token);
                fToken.Close();
                return true;
            }
            else
                return false;
        }
    }
}