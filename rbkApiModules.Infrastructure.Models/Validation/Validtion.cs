using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using rbkApiModules.UIAnnotations;
using System.Threading.Tasks;

namespace rbkApiModules.Infrastructure.Models
{
    public class ValidationResult
    {
        private int? _minLength = null;
        private int? _maxLength = null;
        private bool _required = false;
        private bool _hasErrors = false;

        public ValidationResult(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }

        public string DisplayName { get; set; }

        public bool HasErrors => _hasErrors;

        public void SetMinLength(int value)
        {
            _minLength = value;
            _hasErrors = true;
        }

        public void SetMaxLength(int value)
        {
            _maxLength = value;
            _hasErrors = true;
        }

        public void SetRequired()
        {
            _required = true;
            _hasErrors = true;
        }

        public ValidationError[] Results
        {
            get
            {
                var propertyName = PropertyName;

                if (!String.IsNullOrEmpty(DisplayName))
                {
                    propertyName = DisplayName;
                }

                var results = new List<ValidationError>();

                if (_required)
                {
                    results.Add(new ValidationError(ValidationType.Required, $"'{propertyName}' é obrigatório"));
                }

                if (!_required)
                {
                    if (_minLength != null)
                    {
                        var s = _minLength > 1 ? "s" : "";
                        results.Add(new ValidationError(ValidationType.MinLength, $"'{propertyName}' deve ter no mínimo {_minLength} caracteres"));
                    }
                    else if (_maxLength != null)
                    {
                        var s = _maxLength > 1 ? "s" : "";
                        results.Add(new ValidationError(ValidationType.MinLength, $"'{propertyName}' deve ter no máximo {_maxLength} caracteres"));
                    }
                }

                return results.ToArray();
            }

            // Se for required, ignora min e max
            // Se min e max forem iguais, mensagem de caracteres exatos
            // Se nao for required e tiver min, ignora se for vazio, mas valida min
        }

        public override string ToString()
        {
            var s = Results.Length > 1 ? "s" : "";

            return $"{PropertyName}: {Results.Length} error{s}";
        }
    }

    public class ValidationError
    {
        public ValidationError(ValidationType type, string message)
        {
            Type = type;
            Message = message;
        }

        public ValidationType Type { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{Type}: {Message}";
        }
    }

    public enum ValidationType
    {
        MinLength,
        MaxLength,
        Required,
        ExactLength,
    }

    public static class BaseEntityExtensions
    {
        public static void Validate(this object instance)
        {
            var results = new List<ValidationResult>();

            var properties = instance.GetType().GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(true);

                var propName = property.Name;

                var propValue = property.GetValue(instance);
                var validation = new ValidationResult(propName);

                foreach (object attr in attributes)
                {
                    var minLengthAttribute = attr as MinLengthAttribute;
                    if (minLengthAttribute != null && property.PropertyType == typeof(string))
                    {
                        var attrValue = minLengthAttribute.Length;

                        if (propValue != null && propValue.ToString().Length < attrValue || propValue == null)
                        {
                            validation.SetMinLength(attrValue);
                        }
                    }

                    var maxLengthAttribute = attr as MaxLengthAttribute;
                    if (maxLengthAttribute != null && property.PropertyType == typeof(string))
                    {
                        var attrValue = maxLengthAttribute.Length;

                        if (propValue != null && propValue.ToString().Length > attrValue || propValue == null)
                        {
                            validation.SetMaxLength(attrValue);
                        }
                    }

                    var requiredAttribute = attr as RequiredAttribute;
                    if (requiredAttribute != null)
                    {
                        if (propValue != null && property.PropertyType == typeof(string) || propValue == null)
                        {
                            if (String.IsNullOrEmpty((string)propValue))
                            {
                                validation.SetRequired();
                            }
                        }
                        else
                        {
                            if (propValue == null)
                            {
                                validation.SetRequired();
                            }
                        }
                    }

                    var dialogDataAttribute = attr as DialogDataAttribute;
                    if (dialogDataAttribute != null)
                    {
                        validation.DisplayName = dialogDataAttribute.Name;
                    }
                }

                if (validation.HasErrors)
                {
                    results.Add(validation);
                }
            }

            if (results.Count > 0)
            {
                throw new ModelValidationException(results.ToArray());
            }
        }
    }
}
