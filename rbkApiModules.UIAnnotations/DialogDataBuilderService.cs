using rbkApiModules.Infrastructure;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace rbkApiModules.UIAnnotations
{
    public class DialogDataBuilderService
    {
        public const int InputTextMaxLengh = 50;

        public List<InputControl> Build(Type type)
        {
            var results = new List<InputControl>();

            foreach (var property in type.GetProperties())
            {
                var required = type.GetAttributeFrom<RequiredAttribute>(property);
                var minlen = type.GetAttributeFrom<MinLengthAttribute>(property);
                var maxlen = type.GetAttributeFrom<MaxLengthAttribute>(property);

                var dialogData = type.GetAttributeFrom<DialogDataAttribute>(property);

                if (dialogData != null)
                {
                    string name = Char.ToLower(property.Name[0]) + property.Name.Substring(1);
                    var inputData = new InputControl(name, property.PropertyType, required, minlen, maxlen, dialogData);

                    results.Add(inputData);
                }
            }

            return results;
        }
    }

    public static class ReflectionExtensions
    {
        public static T GetAttributeFrom<T>(this Type type, PropertyInfo property) where T : Attribute
        {
            return (T)property.GetCustomAttributes(typeof(T), false).FirstOrDefault();
        }
    }

    public class InputControl
    {
        private readonly Type _type;

        public InputControl(string propertyName, Type type, RequiredAttribute requiredAttribute, MinLengthAttribute minlengAttribute, 
            MaxLengthAttribute maxlengAttribute, DialogDataAttribute dialogDataAttribute)
        {
            _type = type;

            PropertyName = propertyName;
            Required = requiredAttribute != null;
            MinLength = minlengAttribute?.Length;
            MaxLength = maxlengAttribute?.Length;

            DataSource = new SimpleNamedEntity { Id = ((int)dialogDataAttribute.Source).ToString(), Name = dialogDataAttribute.Source.ToString() };
            DefaultValue = dialogDataAttribute.DefaultValue;
            DependsOn = dialogDataAttribute.DependsOn;
            Group = dialogDataAttribute.Group;
            Mask = dialogDataAttribute.Mask;
            Name = dialogDataAttribute.Name;
            Nullable = dialogDataAttribute.Nullable;

            var control = dialogDataAttribute.ForcedType != DialogControlTypes.Default ? dialogDataAttribute.ForcedType : GetControlType();

            ControlType = new SimpleNamedEntity { Id = ((int)control).ToString(), Name = control.ToString() };
        }

        public string PropertyName { get; set; }
        public SimpleNamedEntity DataSource { get; set; }
        public object DefaultValue { get; set; }
        public string DependsOn { get; set; }
        public SimpleNamedEntity ControlType { get; set; }
        public string Group { get; set; }
        public string Mask { get; set; }
        public string Name { get; set; } 
        public bool Nullable { get; set; }
        public bool Required { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }

        private DialogControlTypes GetControlType()
        {
            if (_type.FullName == typeof(String).FullName)
            {
                if (MaxLength.HasValue && MaxLength.Value <= 100)
                {
                    return DialogControlTypes.InputText;
                }
                else if (MaxLength.HasValue && MaxLength.Value > 100)
                {
                    return DialogControlTypes.InputArea;
                }
                else
                {
                    return DialogControlTypes.InputText;
                }
            }
            else if (_type.FullName == typeof(Boolean).FullName)
            {
                return DialogControlTypes.Numeric;
            }
            else if (_type.FullName == typeof(Int32).FullName || _type.FullName == typeof(Int64).FullName)
            {
                return DialogControlTypes.Numeric;
            }
            else if (_type.FullName == typeof(Single).FullName || _type.FullName == typeof(Double).FullName)
            {
                return DialogControlTypes.Numeric;
            }
            else if (typeof(BaseEntity).IsAssignableFrom(_type))
            {
                return DialogControlTypes.DropDown;
            }
            else if (_type.IsEnum)
            {
                return DialogControlTypes.DropDown;
            }
            else if (_type.FullName.StartsWith("System.Collections.Generic.List`1"))
            {
                return DialogControlTypes.MultiSelect;
            }
            else
            {
                var type = _type;
                throw new NotSupportedException("Type not supported: " + _type.FullName);
            }
        }
    }
}
