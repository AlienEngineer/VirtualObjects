namespace VirtualObjects.CodeGenerators
{
    interface IEntityInfoCodeGeneratorFactory
    {
        IEntityCodeGenerator Make(IEntityInfo info);
    }
}
