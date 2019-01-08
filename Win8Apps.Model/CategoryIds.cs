using System;
using System.ComponentModel;
using System.Reflection;

namespace Win8Apps.Model
{
    public enum CategoryIds
    {
        [Description("Games")]
        Games = 3,
        [Description("Social")]
        Social = 5,
        [Description("Entertainment")]
        Entertainment = 6,
        [Description("Photo")]
        Photo = 7,
        [Description("Music & Video")]
        Music_Video = 8,
        [Description("Sports")]
        Sports = 9,
        [Description("Books & Reference")]
        Books_Reference = 10,
        [Description("News & Weather")]
        News_Weather = 11,
        [Description("Health & Fitness")]
        Health_Fitness = 12,
        [Description("Food & Dining")]
        Food_Dining = 13,
        [Description("Lifestyle")]
        Lifestyle = 14,
        [Description("Shopping")]
        Shopping = 15,
        [Description("Travel")]
        Travel = 16,
        [Description("Finance")]
        Finance = 17,
        [Description("Productivity")]
        Productivity = 18,
        [Description("Tools")]
        Tools = 19,
        [Description("Security")]
        Security = 20,
        [Description("Business")]
        Business = 21,
        [Description("Education")]
        Education = 22,
        [Description("Government")]
        Government = 23,


        Games_Action = 30,
        Games_Adventure = 31,
        Games_Arcade = 32,
        Games_Card = 33,
        Games_Casino = 34,
        Games_Family = 35,
        Games_Kids = 36,
        Games_Music = 37,
        Games_Puzzle = 38,
        Games_Racing = 39,
        Games_Role_playing = 40,
        Games_Shooter = 41,
        Games_Simulation = 42,
        Games_Sports = 43,
        Games_Strategy = 44,

        Music_Video_Music = 80,
        Music_Video_Video = 81,

        Books_Reference_E_reader = 100,
        Books_Reference_Fiction = 101,
        Books_Reference_Non_fiction = 102,
        Books_Reference_Reference = 103,
        Books_Reference_Kids = 104,

        News_Weather_News = 110,
        News_Weather_Weather = 111,


        Security_PC_protection = 201,
        Security_Personal_security = 202
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute == null ? null : attribute.Description;
        }
    }
}