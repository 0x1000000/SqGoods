using System.Collections.Generic;
using System.Text.Json.Serialization;
using SqGoods.Infrastructure;

namespace SqGoods.Models.Fields
{
    [JsonConverter(typeof(TypeDiscriminationConverter<FieldDescriptorModel>))]
    public abstract class FieldDescriptorModel
    {
        public string Id { get; set; } = string.Empty;

        public string Label { get; set; } = string.Empty;

        public string? Note { get; set; }

        public abstract string Type {get;}
    }

    public class StringFieldDescriptorModel : FieldDescriptorModel
    {
        public int? MaxLength { get; set; }

        public override string Type => "text";
    }

    public class BooleanFieldDescriptorModel : FieldDescriptorModel
    {
        public override string Type => "bool";
    }

    public class NumberFieldDescriptorModel : FieldDescriptorModel
    {
        public int? Min { get; set; }

        public int? Max { get; set; }

        public override string Type => "numeric";
    }

    public class SelectFieldDescriptorModel : FieldDescriptorModel
    {
        public List<SelectFieldItemModel>? Items { get; set; }

        public bool Multi { get; set; }

        public bool NullSelection { get; set; }

        public override string Type => "select-static";

        public string IdMember => "id";

        public string TextMember => "title";
    }

    public class SelectFieldItemModel
    {
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
    }
}