using System.Threading;
using EarWorm.Code;
using Microsoft.JSInterop;
using Havit.Blazor.Components.Web.Bootstrap;
using Havit.Blazor.Components.Web;

namespace EarWorm.Shared {
    public partial class Listener {
		public IconBase  _resultIcon = null;
		public class StaffNote {
			public string Note { get; set; }
			public String Color { get; set; }
		}
		public class Staff {
			public string Clef { get; set; }
			public List<StaffNote> Notes { get; set; }
			public Staff() {Notes = new List<StaffNote>();}
	}
		public enum ListenResult {
			Matched,
			Failed,
			Timeout,
			Abandoned,
			RetryLimit,
			Init
		}
		Timer _timer;
		int Time;
		int Max;
		HxModal _modal;
		bool _listening;
		MusicEngine.TestResult _result;
		Mode _mode;
		TaskCompletionSource<MusicEngine.TestResult> _tcs;
		static Listener s_listenerInstance;
		private string _notes;
		IList<int> _noteList;
		int _noteIdx;
		Staff _staffDef;
		DateTime _startTime;


		MusicEngine.TestDefinition _testDef;

		void StartTimer(int max) {
			Max = Time = max * 10;
			_timer = new Timer(async _ =>
			{
				Time -= 1;
				if (Time == 0) {
					// timeout
					_resultIcon = BootstrapIcon.Clock;
					Stop(ListenResult.Timeout);
				}
				await InvokeAsync(StateHasChanged);
			}, null, 0, 100);
		}
		public enum Mode {
			Test,
			Train
		}
		public Listener() {
			Util.Log("construct");
			s_listenerInstance = this;
		}
		public async Task<MusicEngine.TestResult> Show(Mode mode, MusicEngine.TestDefinition testDef) {
			_tcs = new TaskCompletionSource<MusicEngine.TestResult>();
			_testDef = testDef;
			Init(mode);
			_noteList = _testDef.Notes;
			StartTimer(testDef.TimeOut);
			await _modal.ShowAsync();
			StartJSListener();
			_startTime = DateTime.Now;
			return await _tcs.Task;
		}

		private void Init(Mode mode) {
			Util.Log("init");
			_notes = "";
			_mode = mode;
			_noteIdx = 0;
			_result = new MusicEngine.TestResult {
				LR = ListenResult.Abandoned
			};
			_staffDef = new Staff { Clef = "treble" };
			_resultIcon = null;
		}

		private void Note(int n) {
			Util.Log($"note = {n}, listen = {_listening}");
			// to deal with any last note notification left over
			if (!_listening)
				return;
			var result = ListenResult.Init;
			var me = Application.MusicEngine;
			
			// convert midi note number to string n => C#4
			var noteStr = me.GetNoteName(n);
			var newNote = new StaffNote();
			_staffDef.Notes.Add(newNote);
			
			// convert to VexFlow format C#4 => C#/4
			newNote.Note = $"{noteStr[0..^1]}/{noteStr[^1]}";
			newNote.Color = "black";
			// did they play the correct note?
			if (_noteList[_noteIdx] == n) {
				// yes, move to the next one
				_noteIdx++;
				if (_noteIdx == _noteList.Count) {
					// all matched - woo hoo
					_resultIcon = BootstrapIcon.HandThumbsUp;
					result = ListenResult.Matched;
				}
			}
			else {
				// wrong note, we are out of here
				_result.FailedNote = _noteIdx;
				newNote.Color = "red";
				_resultIcon = BootstrapIcon.HandThumbsDown;
				result = ListenResult.Failed;
			}


			Util.JS.InvokeVoidAsync("window.drawStaff", "vf", _staffDef);
			_notes += String.Format("{0} ", noteStr);

			StateHasChanged();
			if (result != ListenResult.Init)
				Stop(result);
		}
		private void StartJSListener() {
			JS.InvokeVoidAsync("pitchStart", "earworm", "NoteHeard");
			//_listening = true;
		}
		private void StopJSListener() {
			_listening = false;
			JS.InvokeVoidAsync("pitchStop");
		}

		// JS listener call back when note heard
		[JSInvokable]
		public static void NoteHeard(int note) {
			s_listenerInstance.Note(note);
		}

		private async void Stop(ListenResult result) {
			if (_timer != null) {
				_timer.Dispose();
				_timer = null;
			}
			StopJSListener();
			_result.LR = result;
			Util.Log("hide1");
			await Task.Delay(1500);
			await _modal.HideAsync();
			Util.Log(string.Format("stop result={0}", result));
		}

		private void Closed() {
			Util.Log($"closed {_listening}");
			if (_listening) {
				_timer.Dispose();
				_timer = null;
				StopJSListener();
			}
			Util.Log(string.Format("set task={0}", _result));
			_result.Time = DateTime.Now - _startTime;
			_tcs.SetResult(_result);

			_tcs = null;
		}

		private void HandleClose() {
			Util.Log($"hc {_listening}");
			Stop(ListenResult.Abandoned);
			StateHasChanged();
		}
		string[] _noNotes = new string[0];
		private void Shown() {
			Util.Log($"shown {_listening}");
			Util.JS.InvokeVoidAsync("window.drawStaff", "vf", _staffDef);
			_listening = true;

		}
		public void Dispose() {
			if (_timer != null) {
				_timer.Dispose();
				_timer = null;
			}
		}
	}

}
