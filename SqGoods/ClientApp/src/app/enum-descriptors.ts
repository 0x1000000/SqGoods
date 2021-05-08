import { SgAttributeType } from 'src/api-client.generated';

export const AttributeTypeDescriptor: {readonly [key: number]: string} =
{
  [SgAttributeType.Boolean]: 'Boolean',
  [SgAttributeType.Integer]: 'Integer',
  [SgAttributeType.Select]: 'Select',
  [SgAttributeType.SubSet]: 'Subset'
};
