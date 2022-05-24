﻿namespace Api.Models;

public class UserCredentials {
    public string username {
        get;
        set;
    }
    public string password {
        get;
        set;
    }
    
    public UserCredentials(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}