using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Channels;
using System.Threading.Tasks;
using luanvanthacsi.Data.Extentions;

namespace luanvanthacsi.Data.Edit
{
    public abstract class EditBaseModel : IValidatableObject
    {
        //public virtual Dictionary<string, Dictionary<string, ISelectItem>> DataSource { get; set; } = new Dictionary<string, Dictionary<string, ISelectItem>>();
        public List<PropertyInfo> InputFields { get; set; } = new List<PropertyInfo>();
        public List<string> FieldsChanged { get; set; } = new List<string>();
        public HashSet<string> DisableInputFields { get; set; } = new HashSet<string>();
        public HashSet<string> RequiredInputFields { get; set; } = new HashSet<string>();
        public event Action OnChange;

        public bool RequiredRefresh { get; set; }
        public bool ReadOnly { get; set; }
        public bool DataChanged { get; set; }
        public bool IsNew { get; set; }
        public bool IsInit { get; set; }

        int hashCode;
        public virtual Task InitFormAsync()
        {
            return Task.FromResult(default(object));
        }

        public void InitForm()
        {
            InitFormAsync();
        }
        public virtual void LoadViewCombobox()
        {

        }
        public virtual Dictionary<string, List<string>> Validate(string nameProperty)
        {
            return null;
        }

        public virtual Dictionary<string, List<string>> ValidateAll(bool breakIfError = false)
        {
            return ValidateInput(InputFields, breakIfError);
        }

        public virtual Dictionary<string, List<string>> ValidateInput(List<PropertyInfo> inputs, bool breakIfError = false)
        {
            Dictionary<string, List<string>> validateMessageStore = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> validateMessage;
            foreach (var field in (inputs ?? InputFields))
            {
                validateMessage = Validate(field.Name);
                if (validateMessage != null && validateMessage.Any())
                {
                    if (breakIfError)
                    {
                        var message = validateMessage.First();
                        validateMessageStore.Add(message.Key, message.Value);
                        return validateMessageStore;
                    }
                    foreach (var messageStore in validateMessage)
                    {
                        if (validateMessageStore.ContainsKey(messageStore.Key))
                        {
                            continue;
                        }
                        validateMessageStore.Add(messageStore.Key, messageStore.Value);
                    }
                }
            }
            return validateMessageStore;
        }


        public virtual string GetCustomClassForField(string nameProperty)
        {
            return string.Empty;
        }

        public override int GetHashCode()
        {
            if (hashCode == 0)
            {
                hashCode = base.GetHashCode();
            }
            return hashCode;
        }

        public virtual void Change(string nameProperty, string value)
        {
            if (DisableInputFields.Contains(nameProperty))
            {
                return;
            }
            var currentValue = this.GetValue(nameProperty);
            if (currentValue?.ToString() == value)
            {
                return;
            }
            this.FieldsChanged.Clear();
            this.SetValue(nameProperty, value);
            FieldsChanged.Add(nameProperty);
            FormChanged();
        }

        public virtual void Change(string nameProperty, object value)
        {
            if (DisableInputFields.Contains(nameProperty))
            {
                return;
            }
            var currentValue = this.GetValue(nameProperty);
            if (Object.Equals(currentValue, value))
            {
                return;
            }
            this.FieldsChanged.Clear();
            this.SetValue(nameProperty, value);
            FieldsChanged.Add(nameProperty);
            FormChanged();
        }

        public void FormChanged()
        {
            DataChanged = true;
            OnChange?.Invoke();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = ValidateAll();
            foreach (var error in errors)
            {
                foreach (var message in error.Value)
                {
                    yield return new ValidationResult(message, new[] { error.Key });
                }
            }
        }

        //public List<SelectItem> GetDataSource<T>(Expression<Func<T>> expression)
        //{
        //    MemberExpression memberExpression = expression.Body as MemberExpression;
        //    if (memberExpression == null)
        //        throw new ArgumentException("'property' không có body");
        //    PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
        //    if (propertyInfo == null
        //        || propertyInfo.ReflectedType == null)
        //        throw new ArgumentException(string.Format("Expression '{0}' can't be cast to an Operand.", expression));
        //    if (DataSource.TryGetValue(propertyInfo.Name, out var value))
        //    {
        //        return value.Values.Select(c => new SelectItem(c.GetKey(), c.GetDisplay())).ToList();
        //    }
        //    return new List<SelectItem>();
        //}
    }
}
