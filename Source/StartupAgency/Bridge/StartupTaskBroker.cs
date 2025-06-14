﻿using System;
using Windows.ApplicationModel;

namespace StartupAgency.Bridge;

/// <summary>
/// Startup task broker
/// </summary>
/// <remarks>
/// This class wraps <see cref="Windows.ApplicationModel.StartupTask"/> which is only available
/// on Windows 10 (version 10.0.14393.0) or greater.
/// </remarks>
public static class StartupTaskBroker
{
	/// <summary>
	/// Determines whether the startup task for a specified AppX package can be enabled.
	/// </summary>
	/// <param name="taskId">Startup task ID</param>
	/// <returns>True if the startup task can be enabled</returns>
	public static bool CanEnable(string taskId)
	{
		if (!PlatformInfo.IsPackaged)
			return false;

		var task = GetStartupTask(taskId);
		return (task.State is not StartupTaskState.DisabledByUser);
	}

	/// <summary>
	/// Determines whether the startup task for a specified AppX package has been enabled.
	/// </summary>
	/// <param name="taskId">Startup task ID</param>
	/// <returns>True if the startup task has been enabled</returns>
	public static bool IsEnabled(string taskId)
	{
		if (!PlatformInfo.IsPackaged)
			return false;

		var task = GetStartupTask(taskId);
		return (task.State is StartupTaskState.Enabled);
	}

	/// <summary>
	/// Enables the startup task for a specified AppX package.
	/// </summary>
	/// <param name="taskId">Startup task ID</param>
	/// <returns>True if the startup task is enabled</returns>
	public static bool Enable(string taskId)
	{
		if (!PlatformInfo.IsPackaged)
			return false;

		var task = GetStartupTask(taskId);
		switch (task.State)
		{
			case StartupTaskState.Enabled:
				return true;

			case StartupTaskState.Disabled:
				var result = task.RequestEnableAsync().AsTask().Result;
				return (result is StartupTaskState.Enabled);

			default:
				return false;
		}
	}

	/// <summary>
	/// Disables the startup task for a specified AppX package.
	/// </summary>
	/// <param name="taskId">Startup task ID</param>
	public static void Disable(string taskId)
	{
		if (!PlatformInfo.IsPackaged)
			return;

		var task = GetStartupTask(taskId);
		switch (task.State)
		{
			case StartupTaskState.Enabled:
				task.Disable();
				break;
		}
	}

	private static StartupTask GetStartupTask(string taskId)
	{
		if (string.IsNullOrWhiteSpace(taskId))
			throw new ArgumentNullException(nameof(taskId));

		return StartupTask.GetAsync(taskId).AsTask().Result;
	}
}