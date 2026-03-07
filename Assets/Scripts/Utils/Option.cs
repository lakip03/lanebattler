using System;
using System.Collections.Generic;


public readonly struct Option<T>
{
    private readonly bool hasValue;
    private readonly T value;

    public bool HasValue => hasValue;

    private Option(bool hasValue, T value)
    {
        this.hasValue = hasValue;
        this.value = value;
    }

    public static Option<T> Some(T value) => new(true, value);
    public static Option<T> None() => new(false, default);

    public void DoIfSome(Action<T> action)
    {
        if (hasValue) action(value);
    }

    public void DoIfNone(Action action)
    {
        if (!hasValue) action();
    }

    public void Match(Action<T> ifSome, Action ifNone)
    {
        if (hasValue) ifSome(value);
        else ifNone();
    }

    public TNew Match<TNew>(Func<T, TNew> ifSome, Func<TNew> ifNone) => hasValue
        ? ifSome(value)
        : ifNone();

    public T GetValueOrDefault(T whenNone) => hasValue
        ? value
        : whenNone;

    public T GetValueOrDefault(Func<T> whenNone) => hasValue
        ? value
        : whenNone();

    public Option<TNew> Map<TNew>(Func<T, TNew> mapping) => hasValue
        ? Option<TNew>.Some(mapping(value))
        : Option<TNew>.None();

    public IEnumerable<T> AsEnumerable()
    {
        if (hasValue) yield return value;
    }

    public static implicit operator Option<T>(T value) => value != null
        ? Some(value)
        : None();
}