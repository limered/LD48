using System;
using System.Collections.Generic;
using Godot;

namespace IsThisATiger2.Empty.Utils;

public partial class EventBus : Node
{
    private readonly Dictionary<Type, List<object>> _registrations = new(); 
    
    public void Register<T>(Action<T> action) where T : new()
    {
        if (!_registrations.ContainsKey(typeof(T)))
        {
            _registrations.Add(typeof(T), new List<object>());    
        }
        _registrations[typeof(T)].Add(action);
    }
    
    public void Emit<T>(T emitted)
    {
        if (!_registrations.ContainsKey(typeof(T))) return;
        
        foreach (Action<T> action in _registrations[typeof(T)])
        {
            action(emitted);
        }
    }

    public void Deregister<T>(Action<T> action)
    {
        if (!_registrations.ContainsKey(typeof(T))) return;
        _registrations[typeof(T)].Remove(action);
    }
}