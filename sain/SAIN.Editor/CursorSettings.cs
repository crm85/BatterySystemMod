using System;
using System.Reflection;
using UnityEngine;

namespace SAIN.Editor;

internal static class CursorSettings
{
	private static bool _displayingWindow;

	private static PropertyInfo _curLockState;

	private static PropertyInfo _curVisible;

	private static int _previousCursorLockState;

	private static bool _previousCursorVisible;

	private static bool _obsoleteCursor;

	public static bool DisplayingWindow
	{
		get
		{
			return _displayingWindow;
		}
		set
		{
			if (_displayingWindow == value)
			{
				return;
			}
			_displayingWindow = value;
			if (_displayingWindow)
			{
				if (_curLockState != null)
				{
					_previousCursorLockState = (_obsoleteCursor ? Convert.ToInt32((bool)_curLockState.GetValue(null, null)) : ((int)_curLockState.GetValue(null, null)));
					_previousCursorVisible = (bool)_curVisible.GetValue(null, null);
				}
			}
			else if (!_previousCursorVisible || _previousCursorLockState != 0)
			{
				SetUnlockCursor(_previousCursorLockState, _previousCursorVisible);
			}
		}
	}

	public static void InitCursor()
	{
		Type typeFromHandle = typeof(Cursor);
		_curLockState = typeFromHandle.GetProperty("lockState", BindingFlags.Static | BindingFlags.Public);
		_curVisible = typeFromHandle.GetProperty("visible", BindingFlags.Static | BindingFlags.Public);
		if (_curLockState == null && _curVisible == null)
		{
			_obsoleteCursor = true;
			_curLockState = typeof(Screen).GetProperty("lockCursor", BindingFlags.Static | BindingFlags.Public);
			_curVisible = typeof(Screen).GetProperty("showCursor", BindingFlags.Static | BindingFlags.Public);
		}
	}

	public static void SetUnlockCursor(int lockState, bool cursorVisible)
	{
		if (_curLockState != null)
		{
			if (_obsoleteCursor)
			{
				_curLockState.SetValue(null, Convert.ToBoolean(lockState), null);
			}
			else
			{
				_curLockState.SetValue(null, lockState, null);
			}
			_curVisible.SetValue(null, cursorVisible, null);
		}
	}
}
