using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace nnclmobileappfiap
{
    public class Professor
    {
        string id;
        string rm;
        string nome;
        string email;
        bool aprovado;
        // TODO IsAprovado retornando a flag <- localDB

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [JsonProperty(PropertyName = "rm")]
        public string Rm
        {
            get { return rm; }
            set { rm = value; }
        }

        [JsonProperty(PropertyName = "email")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [JsonProperty(PropertyName = "nome")]
        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        [JsonProperty(PropertyName = "aprovado")]
        public bool Aprovado
        {
            get { return aprovado; }
            set { aprovado = value; }
        }

        // TODO Add version
    }
}
