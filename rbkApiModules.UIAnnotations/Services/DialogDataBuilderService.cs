using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace rbkApiModules.UIAnnotations
{
    public class DialogDataBuilderService
    {
        public const int InputTextMaxLengh = 50;

        public List<FormGroup> Build(Type type, OperationType operation)
        {
            var results = new List<FormGroup>();

            var allInputs = new List<InputControl>();

            foreach (var property in type.GetProperties())
            {
                var required = type.GetAttributeFrom<RequiredAttribute>(property);
                var minlen = type.GetAttributeFrom<MinLengthAttribute>(property);
                var maxlen = type.GetAttributeFrom<MaxLengthAttribute>(property);

                var dialogData = type.GetAttributeFrom<DialogDataAttribute>(property);

                if (dialogData != null && (operation == dialogData.Operation || dialogData.Operation == OperationType.CreateAndUpdate))
                {
                    var name = Char.ToLower(property.Name[0]) + property.Name.Substring(1);

                    if (dialogData.Source == DataSource.ChildForm)
                    {
                        allInputs.AddRange(Build(property.PropertyType, operation).SelectMany(x => x.Controls));
                    }
                    else
                    {
                        var inputData = new InputControl(name, property.PropertyType, required, minlen, maxlen, dialogData);
                        allInputs.Add(inputData);
                    }
                }
            }

            var groups = allInputs.GroupBy(x => x.Group);

            foreach (var groupedData in groups)
            {
                var group = new FormGroup();
                group.Name = groupedData.Key;
                group.Controls.AddRange(groupedData);

                results.Add(group);
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

            PropertyName = String.IsNullOrEmpty(dialogDataAttribute.OverridePropertyName) ? propertyName : dialogDataAttribute.OverridePropertyName;
            Required = requiredAttribute != null;
            MinLength = minlengAttribute != null ? (int?)minlengAttribute.Length : null;
            MaxLength = maxlengAttribute != null ? (int?)maxlengAttribute.Length : null;

            DataSource = dialogDataAttribute.Source != UIAnnotations.DataSource.None
                ? new SimpleNamedEntity { Id = ((int)dialogDataAttribute.Source).ToString(), Name = dialogDataAttribute.Source.ToString() }
                : null;
            DefaultValue = dialogDataAttribute.DefaultValue;
            DependsOn = dialogDataAttribute.DependsOn;
            Group = dialogDataAttribute.Group;
            Mask = dialogDataAttribute.Mask;
            Unmask = dialogDataAttribute.Unmask;
            CharacterPattern = dialogDataAttribute.CharacterPattern;
            FileAccept = dialogDataAttribute.FileAccept;
            Name = dialogDataAttribute.Name;
            IsVisible = dialogDataAttribute.IsVisible ? null : (bool?)false;
            ExcludeFromResponse = dialogDataAttribute.ExcludeFromResponse;
            SourceName = dialogDataAttribute.SourceName;
            EntityLabelPropertyName = dialogDataAttribute.EntityLabelPropertyName;

            var control = dialogDataAttribute.ForcedType != DialogControlTypes.Default ? dialogDataAttribute.ForcedType : GetControlType();

            ControlType = new SimpleNamedEntity { Id = ((int)control).ToString(), Name = control.ToString() };

            if (control == DialogControlTypes.DropDown || control == DialogControlTypes.MultiSelect || control == DialogControlTypes.LinkedDropDown)
            {
                PropertyName = FixDropDownName(propertyName);
                ShowFilter = dialogDataAttribute.ShowFilter;
                FilterMatchMode = dialogDataAttribute.FilterMatchMode;
            }

            if (control == DialogControlTypes.TextArea)
            {
                TextAreaRows = dialogDataAttribute.TextAreaRows > 0 ? dialogDataAttribute.TextAreaRows: 5;
            }

            if (_type.IsEnum)
            {
                Data = EnumToSimpleNamedList(_type);
            }
        }

        public SimpleNamedEntity ControlType { get; set; }
        public SimpleNamedEntity DataSource { get; set; }
        public string SourceName { get; set; }
        public string PropertyName { get; set; }

        public string Name { get; set; }
        public object DefaultValue { get; set; }
        public string Group { get; set; }
        public bool? IsVisible { get; set; }
        public bool? ExcludeFromResponse { get; set; }

        public string DependsOn { get; set; }

        public int? TextAreaRows { get; set; }
        
        public string Mask { get; set; }
        public bool? Unmask { get; set; }
        public string CharacterPattern { get; set; }

        public string FileAccept { get; set; }

        public bool? ShowFilter { get; set; }
        public string FilterMatchMode{ get; set; }

        public bool Required { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public List<SimpleNamedEntity<int>> Data { get; set; }

        public string EntityLabelPropertyName { get; set; } 

        private DialogControlTypes GetControlType()
        {
            if (_type.FullName == typeof(String).FullName)
            {
                if (MaxLength.HasValue && MaxLength.Value <= 100)
                {
                    return DialogControlTypes.Text;
                }
                else if (MaxLength.HasValue && MaxLength.Value > 100)
                {
                    return DialogControlTypes.TextArea;
                }
                else
                {
                    return DialogControlTypes.Text;
                }
            }
            else if (_type.FullName == typeof(Boolean).FullName || _type.FullName == typeof(Boolean?).FullName)
            {
                return DialogControlTypes.CheckBox;
            }
            else if (_type.FullName == typeof(Int32).FullName || _type.FullName == typeof(Int64).FullName ||
                     _type.FullName == typeof(Int32?).FullName || _type.FullName == typeof(Int64?).FullName)
            {
                return DialogControlTypes.Number;
            }
            else if (_type.FullName == typeof(Single).FullName || _type.FullName == typeof(Double).FullName ||
                     _type.FullName == typeof(Single?).FullName || _type.FullName == typeof(Double?).FullName)
            {
                return DialogControlTypes.Number;
            }
            else if (_type.FullName == typeof(DateTime).FullName || _type.FullName == typeof(DateTime?).FullName)
            {
                return DialogControlTypes.Calendar;
            }
            else if (typeof(BaseEntity).IsAssignableFrom(_type))
            {
                PropertyName = FixDropDownName(PropertyName);
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
            else if (_type.FullName == typeof(Guid).FullName || _type.FullName == typeof(Guid?).FullName)
            {
                return DialogControlTypes.Text;
            }
            else
            {
                throw new NotSupportedException("Type not supported: " + _type.FullName);
            }
        }

        private string FixDropDownName(string propertyName)
        {
            return propertyName.EndsWith("Id") ? propertyName.Substring(0, propertyName.Length - 2) : propertyName;
        }

        private List<SimpleNamedEntity<int>> EnumToSimpleNamedList(Type type)
        {
            var results = new List<SimpleNamedEntity<int>>();
            var names = Enum.GetNames(type);
            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i];
                var field = type.GetField(name);
                var fds = field.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
                var id = (int)Enum.Parse(type, name);

                if (fds != null)
                {
                    results.Add(new SimpleNamedEntity<int> { Id = id, Name = (fds as DescriptionAttribute).Description });
                }
                else
                {
                    results.Add(new SimpleNamedEntity<int> { Id = id, Name = field.Name });
                }
            }

            return results;
        }
    }
}
