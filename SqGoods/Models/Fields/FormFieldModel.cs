namespace SqGoods.Models.Fields
{
    public class FormFieldModel
    {
        public FieldDescriptorModel? Descriptor { get; set; }
        public bool Mandatory { get; set; }
        public bool Disabled { get; set; }
    }
}