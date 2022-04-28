using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace modelo.ViewModel
{
    public class CorreoMicrosoftViewModels
    {
        public List<ValueCorreo> value { get; set; }
    }
    public class ValueCorreo
    {
        public string id { get; set; }
        public string receivedDateTime { get; set; }
        public string subject { get; set; }
        public string bodyPreview { get; set; }
        public bool isRead { get; set; }
        public BodyCorreo body { get; set; }
        public FromCorreo from { get; set; }
    }
    public class FromCorreo
    {
        public EmailAddressCorreo emailAddress { get; set; }
    }
    public class EmailAddressCorreo
    {
        public string name { get; set; }
        public string address { get; set; }
    }
    public class BodyCorreo
    {
        public string contentType { get; set; }
        public string content { get; set; }
    }
}