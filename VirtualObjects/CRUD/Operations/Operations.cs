namespace VirtualObjects.CRUD.Operations
{
    class Operations : IOperations
    {
        public IOperation InsertOperation { get; set; }
        public IOperation UpdateOperation { get; set; }
        public IOperation DeleteOperation { get; set; }
        public IOperation GetOperation { get; set; }
        public IOperation GetVersionOperation { get; set; }
        public IOperation CountOperation { get; set; }
    }
}
