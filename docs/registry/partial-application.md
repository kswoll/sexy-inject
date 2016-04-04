# Partial Application

One of the more unique features of SexyInject is that you can request a factory that allows injection of dependencies for arguments that were ommitted via default parameters. In other words, if you invoke a constructor and omit certain parameters and allow the default values be used (whether via named arguments or, truncating the argument list) then those default values will instead be injected via the registry.  

The use-case is a conventional front-end in which you constantly new up forms and generally need to pass in state to that new form (for example, the selected item for which you want to show a detail view).  Conventional DI works great for scenarios where the central class could not receive any paramterized state, like ASP.NET, both MVC and WebAPI.  Technically state is passed in, but it's on top of an infrastructure in which that state is available via properties on the controller.  This works -- but it's often not how you really want to code these interactions.  Often, you just want to create a new window and simultaneously inject the dependencies that are available via the DI framework, while still passing in some local state, such as the selected item.

To enable this feature:

    registry.RegisterPartialApplication();
    
Then, supposing you have a class such as:

    public class ItemWindow
    {
        public Item Item { get; }
        public IEmailService EmailService { get; }
        
        public ItemWindow(Item item, IEmailService emailService = null)
        {
            Item = item;
            EmailService = emailService;
        }
    }
    
Then you can instantiate this via:

    registry.Construct(_ => new ItemWindow(item));
    
In a more DI friendly scenario, just ask for either a `Func<Func<T>>`, where `T` is the type you are trying to create, or use the included delegate `PartialConstructor<T>`, which scans a bit better.

