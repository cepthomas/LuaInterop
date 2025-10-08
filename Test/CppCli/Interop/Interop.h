///// Warning - this file is created by gen_interop.lua - do not edit. /////

#pragma once
#include "cliex.h"

using namespace System;
using namespace System::Collections::Generic;

//============= interop C => Cpp/CLI callback payload =============//

//--------------------------------------------------------//
public ref class LogArgs : public EventArgs
{
public:
    /// <summary>Log message</summary>
    property String^ msg;
    /// <summary>Unused</summary>
    property int ret;
    /// <summary>Constructor.</summary>
    LogArgs(const char* msg)
    {
        this->msg = gcnew String(msg);
    }
};

//--------------------------------------------------------//
public ref class NotificationArgs : public EventArgs
{
public:
    /// <summary>A number</summary>
    property int arg_I;
    /// <summary>Some text</summary>
    property String^ arg_S;
    /// <summary>boooooool</summary>
    property bool arg_B;
    /// <summary>numero/doublo</summary>
    property double arg_N;
    /// <summary>Back at you</summary>
    property int ret;
    /// <summary>Constructor.</summary>
    NotificationArgs(int arg_I, const char* arg_S, bool arg_B, double arg_N)
    {
        this->arg_I = arg_I;
        this->arg_S = gcnew String(arg_S);
        this->arg_B = arg_B;
        this->arg_N = arg_N;
    }
};


//----------------------------------------------------//
public ref class Interop : CliEx
{

//============= Cpp/CLI => interop C functions =============//
public:

    /// <summary>Setup</summary>
    /// <param name="opt">Option</param>
    /// <returns>Script return</returns>
    int Setup(int opt);

    /// <summary>DoCommand</summary>
    /// <param name="cmd">Specific command</param>
    /// <param name="arg_B">bool argument</param>
    /// <param name="arg_I">int argument</param>
    /// <param name="arg_N">number/double argument</param>
    /// <param name="arg_S">string argument</param>
    /// <returns>Script return</returns>
    double DoCommand(String^ cmd, bool arg_B, int arg_I, double arg_N, String^ arg_S);

    /// <summary>DoCommand</summary>
    /// <param name="cmd">Specific command</param>
    /// <param name="arg">int argument</param>
    /// <returns>Script return</returns>
    String^ DoCommand(String^ cmd, int arg);

//============= interop C => Cpp/CLI callback functions =============//
public:
    static event EventHandler<LogArgs^>^ Log;
    static void Notify(LogArgs^ args) { Log(nullptr, args); }

    static event EventHandler<NotificationArgs^>^ Notification;
    static void Notify(NotificationArgs^ args) { Notification(nullptr, args); }


//============= Infrastructure =============//
public:
    /// <summary>Initialize and execute script file.</summary>
    /// <param name="scriptFn">The script to load.</param>
    /// <param name="luaPath">LUA_PATH components</param>
    void RunScript(String^ scriptFn, String^ luaPath);

    /// <summary>Initialize and execute a chunk of lua code.</summary>
    /// <param name="code">The lua code to load.</param>
    /// <param name="luaPath">LUA_PATH components</param>
    void RunChunk(String^ code, String^ luaPath);
};
