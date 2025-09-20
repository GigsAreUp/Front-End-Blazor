namespace MusicHFE2.Services
{
    using MusicHFE2.Models;
    // Services/StateService.cs
    using System;
    using System.Collections.Generic;

    public class StateService
    {
        public User? User { get; private set; }
        public string UserType { get; private set; } = "seller";
        public List<string> Tags { get; } = new List<string> { "Web Development", "Graphic Design", "Content Writing" };
        public List<string> SelectedTags { get; } = new List<string>();
        public List<Slot> Slots { get; } = new List<Slot>
    {
        new Slot { Id = Guid.NewGuid(), Date = "2023-10-15", Time = "Morning", Price = 50, Available = true },
        new Slot { Id =  Guid.NewGuid(), Date = "2023-10-15", Time = "Evening", Price = 60, Available = true },
        new Slot { Id =  Guid.NewGuid(), Date = "2023-10-16", Time = "Morning", Price = 55, Available = false }
    };
        public List<Order> Orders { get; } = new List<Order>
    {
        new Order { Id = 1, Seller = "John Doe", Service = "Web Development", Date = "2023-10-15", Time = "Morning", Status = "Pending", Price = 50 },
        new Order { Id = 2, Seller = "Jane Smith", Service = "Graphic Design", Date = "2023-10-10", Time = "Evening", Status = "Confirmed", Price = 75 }
    };

        public event Action? OnChange;

        public void SetUser(User? user)
        {
            User = user;
            NotifyStateChanged();
        }

        public void SetUserType(string userType)
        {
            UserType = userType;
            NotifyStateChanged();
        }

        // No need for set methods for lists since they are modified directly and Notify called

        public void NotifyStateChanged() => OnChange?.Invoke();
    }
}
