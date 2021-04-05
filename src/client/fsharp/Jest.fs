module Jest

open Fable.Core
open System.Text.RegularExpressions

[<Global("it")>]
let it (msg: string) (f: unit -> unit) : unit = jsNative

[<Global>]
let describe (msg: string) (f: unit -> unit) : unit = jsNative

[<AllowNullLiteral>]
type Expect =

    /// If you know how to test something, ```.not``` lets you test its opposite.
    abstract not : Expect with get, set
    /// Use resolves to unwrap the value of a fulfilled promise so any other matcher can be chained. If the promise is rejected the assertion fails.
    abstract resolves : Expect with get, set
    /// Use ```.rejects``` to unwrap the reason of a rejected promise so any other matcher can be chained. If the promise is fulfilled the assertion fails.
    abstract rejects : Expect with get, set
    /// Use ```.toEqual``` when you want to check that two objects have the same value. This matcher recursively checks the equality of all fields, rather than checking for object identity—this is also known as "deep equal".
    abstract toEqual : 'a -> unit
    /// toBe just checks that a value is what you expect. It uses === to check strict equality.
    abstract toBe : 'a -> unit
    /// Use ```.toBeCalled``` to ensure that a mock function got called.
    abstract toBeCalled : unit -> unit
    /// Use ```.toHaveBeenCalled``` to ensure that a mock function got called.
    abstract toHaveBeenCalled : unit -> unit
    /// Use ```.toHaveBeenCalledTimes``` to ensure that a mock function got called exact number of times.
    abstract toHaveBeenCalledTimes : int -> unit
    ///  Use ```.toBeCalledWith``` to ensure that a mock function was called with specific arguments.
    abstract toBeCalledWith : 'a -> unit

    abstract toBeCalledWith : 'a * 'b -> unit
    abstract toBeCalledWith : 'a * 'b * 'c -> unit
    abstract toBeCalledWith : 'a * 'b * 'c * 'd -> unit
    abstract toBeCalledWith : 'a * 'b * 'c * 'd * 'e -> unit
    /// If you have a mock function, you can use ```.lastCalledWith``` to test what arguments it was last called with
    [<Emit("$0.lastCalledWith()")>]
    abstract lastCalledWith : unit -> unit

    abstract lastCalledWith : 'a -> unit
    abstract lastCalledWith : 'a * 'b -> unit
    abstract lastCalledWith : 'a * 'b * 'c -> unit
    abstract lastCalledWith : 'a * 'b * 'c * 'd -> unit
    abstract lastCalledWith : 'a * 'b * 'c * 'd * 'e -> unit
    abstract toBeCloseTo : float -> int -> unit
    /// Use .toBeDefined to check that a variable is not undefined
    abstract toBeDefined : unit -> unit
    /// Use ```.toBeFalsy``` when you don't care what a value is, you just want to ensure a value is false in a boolean context
    abstract toBeFalsy : unit -> unit
    /// To compare floating point numbers, you can use toBeGreaterThan.
    abstract toBeGreaterThan : int -> unit

    abstract toBeGreaterThan : float -> unit
    /// To compare floating point numbers, you can use toBeGreaterThanOrEqual.
    abstract toBeGreaterThanOrEqual : int -> unit

    abstract toBeGreaterThanOrEqual : float -> unit
    /// To compare floating point numbers, you can use toBeLessThan.
    abstract toBeLessThan : int -> unit

    abstract toBeLessThan : float -> unit
    /// To compare floating point numbers, you can use toBeLessThanOrEqual.
    abstract toBeLessThanOrEqual : int -> unit

    abstract toBeLessThanOrEqual : float -> unit
    /// ```.toBeNull()``` is the same as ```.toBe(null)``` but the error messages are a bit nicer.
    abstract toBeNull : unit -> unit
    /// Use ```.toBeTruthy``` when you don't care what a value is, you just want to ensure a value is true in a boolean context.
    abstract toBeTruthy : unit -> unit
    /// Use ```.toBeUndefined``` to check that a variable is undefined
    abstract toBeUndefined : unit -> unit
    /// Use ```.toContain``` when you want to check that an item is in an array.
    abstract toContain : 'a -> unit
    /// Use ```.toContainEqual``` when you want to check that an item is in a list. For testing the items in the list, this matcher recursively checks the equality of all fields, rather than checking for object identity.
    abstract toContainEqual : 'a -> unit
    /// Use ```.toHaveLength``` to check that an object has a ```.length``` property and it is set to a certain numeric value.
    /// This is especially useful for checking arrays or strings size.
    abstract toHaveLength : int -> unit
    /// Use ```.toMatch``` to check that a string matches a regular expression.
    abstract toMatch : Regex -> unit

    abstract toMatch : string -> unit
    /// Use ```.toMatchObject``` to check that a JavaScript object matches a subset of the properties of an object. It will match received objects with properties that are not in the expected object.
    /// You can also pass an array of objects, in which case the method will return true only if each object in the received array matches (in the toMatchObject sense described above) the corresponding object in the expected array. This is useful if you want to check that two arrays match in their number of elements, as opposed to arrayContaining, which allows for extra elements in the received array.
    abstract toMatchObject : obj -> unit
    /// Use ```.toHaveProperty``` to check if property at provided reference keyPath exists for an object. For checking deeply nested properties in an object use dot notation for deep references.
    /// Optionally, you can provide a value to check if it's equal to the value present at keyPath on the target object. This matcher uses 'deep equality' (like toEqual()) and recursively checks the equality of all fields.
    abstract toHaveProperty : string -> 'a option -> unit
    /// This ensures that a value matches the most recent snapshot. Check out the Snapshot Testing guide for more information.
    /// You can also specify an optional snapshot name. Otherwise, the name is inferred from the test.
    abstract toMatchSnapshot : unit -> unit

    abstract toMatchSnapshot : string -> unit
    /// Use ```.toThrow``` to test that a function throws when it is called.
    /// If you want to test that a specific error gets thrown, you can provide an argument to toThrow. The argument can be a string for the error message, a class for the error, or a regex that should match the error.
    abstract toThrow : unit -> unit

    abstract toThrow : System.Exception -> unit
    abstract toThrow : string -> unit
    abstract toThrow : Regex -> unit
    /// Use ```.toThrowErrorMatchingSnapshot``` to test that a function throws an error matching the most recent snapshot when it is called.
    abstract toThrowErrorMatchingSnapshot : unit -> unit

[<Global>]
let expect : obj -> Expect = jsNative
