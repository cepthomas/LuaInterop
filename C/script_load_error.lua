-- Script with syntax errors to detect on loadfile.

local gen = require("luainterop")

---@diagnostic disable-next-line: undefined-global
bad_statement_oops

ts = gen.get_timestamp()

print(ts)
