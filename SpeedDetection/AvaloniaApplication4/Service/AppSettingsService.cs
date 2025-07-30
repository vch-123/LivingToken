using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication4.Service
{
    public class AppSettingsService:INotifyPropertyChanged
    {

        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public AppSettingsService()
        {
            _configuration=App.Services.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
        }
        private bool _useAbsoluteValue = false;
        public bool UseAbsoluteValue
        {
            get=> _useAbsoluteValue;
            set { _useAbsoluteValue=value;OnPropertyChanged(); }
        }

        private string _sidebarBackground = "#807060";
        public string SidebarBackground
        {
            get => _sidebarBackground;
            set { _sidebarBackground = value; OnPropertyChanged(); }
        }

        private string _contentBackground = "#E0D9C6";
        public string ContentBackground
        {
            get => _contentBackground;
            set { _contentBackground = value; OnPropertyChanged(); }
        }

        private string _foreground = "#E0D9C6";
        public string Foreground
        {
            get => _foreground;
            set { _foreground = value; OnPropertyChanged(); }
        }


        public string BrokerIp => _configuration["MqttConfig:BrokerIp"] ?? "";

        public int Port
        {
            get
            {
                int.TryParse(_configuration["MqttConfig:Port"], out var port);
                return port;
            }
        }

        public string ClientId => _configuration["MqttConfig:ClientId"] ?? "";

        public List<string> Topics => _configuration.GetSection("MqttConfig:Topics").Get<List<string>>() ?? new List<string>();







        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        
    }

    public class MqttConfigDto
    {
        public string BrokerIp { get; set; } = "";
        public int Port { get; set; }
        public string ClientId { get; set; } = "";
        public List<string> Topics { get; set; } = new();
    }

}
