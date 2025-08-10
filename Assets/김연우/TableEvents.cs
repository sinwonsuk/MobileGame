public struct TableAddedEvent : IEvent
{
    public CustomerTable table;
    public TableAddedEvent(CustomerTable table) { this.table = table; }
}

public struct TableRemovedEvent : IEvent
{
    public CustomerTable table;
    public TableRemovedEvent(CustomerTable table) { this.table = table; }
}
