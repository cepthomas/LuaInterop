///// Warning - this file is created by gen_interop.lua - do not edit. 2025-03-05 08:53:28 /////

#pragma once
#include "Core.h"

using namespace System;
using namespace System::Collections::Generic;

namespace Interop
{

//============= C => C# callback payload .h =============//

//--------------------------------------------------------//
public ref class LogArgs : public EventArgs
{
public:
    /// <summary>Log level</summary>
    property int level;
    /// <summary>Log message</summary>
    property String^ msg;
    /// <summary>Constructor.</summary>
    LogArgs(int level, const char* msg)
    {
        this->level = level;
        this->msg = gcnew String(msg);
    }
};

//--------------------------------------------------------//
public ref class NotificationArgs : public EventArgs
{
public:
    /// <summary>A number</summary>
    property int num;
    /// <summary>Some text</summary>
    property String^ text;
    /// <summary>Constructor.</summary>
    NotificationArgs(int num, const char* text)
    {
        this->num = num;
        this->text = gcnew String(text);
    }
};


//----------------------------------------------------//
public ref class HostInterop : Core
{

//============= C# => C functions .h =============//
public:

    /// <summary>Setup</summary>
    /// <param name="opt">Option</param>
    /// <returns>Script return</returns>
    int Setup(int opt);

    /// <summary>DoCommand</summary>
    /// <param name="cmd">Specific command</param>
    /// <param name="arg">Optional argument</param>
    /// <returns>Script return</returns>
    String^ DoCommand(String^ cmd, String^ arg);

//============= C => C# callback functions =============//
public:
    static event EventHandler<LogArgs^>^ Log;
    static void Notify(LogArgs^ args) { Log(nullptr, args); }

    static event EventHandler<NotificationArgs^>^ Notification;
    static void Notify(NotificationArgs^ args) { Notification(nullptr, args); }


//============= Infrastructure .h =============//
public:
    /// <summary>Initialize and execute.</summary>
    /// <param name="scriptFn">The script to load.</param>
    /// <param name="luaPath">LUA_PATH components</param>
    void Run(String^ scriptFn, List<String^>^ luaPath);
};

}
