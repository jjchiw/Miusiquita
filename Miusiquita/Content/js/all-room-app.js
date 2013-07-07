/*global ko */
(function() {
	'use strict';

	var ENTER_KEY = 13;

	// a custom binding to handle the enter key (could go in a separate library)
	ko.bindingHandlers.enterKey = {
		init: function( element, valueAccessor, allBindingsAccessor, data ) {
			var wrappedHandler, newValueAccessor;

			// wrap the handler with a check for the enter key
			wrappedHandler = function( data, event ) {
				if ( event.keyCode === ENTER_KEY ) {
				    valueAccessor().call(this, data, event);
				}
			};

			// create a valueAccessor with the options that we would want to pass to the event binding
			newValueAccessor = function() {
				return {
					keyup: wrappedHandler
				};
			};

			// call the real event binding's init function
			ko.bindingHandlers.event.init( element, newValueAccessor, allBindingsAccessor, data );
		}
	};

	// wrapper to hasfocus that also selects text and applies focus async
	ko.bindingHandlers.selectAndFocus = {
		init: function( element, valueAccessor, allBindingsAccessor ) {
			ko.bindingHandlers.hasfocus.init( element, valueAccessor, allBindingsAccessor );
			ko.utils.registerEventHandler( element, 'focus', function() {
				element.focus();
			});
		},
		update: function( element, valueAccessor ) {
			ko.utils.unwrapObservable( valueAccessor() ); // for dependency
			// ensure that element is visible before trying to focus
			setTimeout(function() {
				ko.bindingHandlers.hasfocus.update( element, valueAccessor );
			}, 0);
		}
	};

	// represent a single todo item
	var Room = function (id, name, dropboxPath) {
	    this.Id = id;
		this.name = ko.observable( name );
		this.dropboxPath = ko.observable(dropboxPath);
	};

	// our main view model
	var ViewModel = function( room ) {
	    var self = this;
	    

		// map array of passed in rooms to an observableArray of Todo objects
		self.rooms = ko.observableArray(ko.utils.arrayMap( room, function( room ) {
			return new Room(room.Id, room.Name, room.DropboxPath );
		}));

	    // store the new todo value being entered
		self.name = ko.observable();
		self.dropboxPath = ko.observable();

		self.currentRoom = null;
		self.editing = ko.observable();
		self.editing(false)
		

		// add a new todo, when enter key is pressed
		self.add = function () {
		    var name = self.name().trim();
		    var dropboxPath = self.dropboxPath().trim();
		    if (name && dropboxPath) {
		        var room = new Room("", name, dropboxPath);
			    $.post("/rooms", room, function (data) {
			        self.rooms.push(new Room(data.Id, data.Name, data.DropboxPath));
			        self.name('');
			        self.dropboxPath('');
			    }, "json");
			}
		};

		self.update = function () {
		    self.currentRoom.name(self.name().trim());
		    self.currentRoom.dropboxPath(self.dropboxPath().trim());
		    $.ajax({
		        url: '/rooms',
		        type: 'PUT',
		        data: self.currentRoom,
		        success: function (response) {
		            self.name("");
		            self.dropboxPath("");
		            self.currentRoom = null;
		            self.editing(false);
		        }
		    });
		    self.editing(false);
		}

		// remove a single todo
		self.remove = function( room ) {
		    $.ajax({
		        url: '/rooms',
		        type: 'DELETE',
		        data: room,
		        success: function (response) {
		            self.rooms.remove(room);
		        }
		    });
		};

		// edit an item
		self.editItem = function (room) {
		    self.currentRoom = room;
		    self.name(self.currentRoom.name());
		    self.dropboxPath(self.currentRoom.dropboxPath());
		    self.editing(true);
		};

		self.cancelEditing = function () {
		    self.name("");
		    self.dropboxPath("");
		    self.currentRoom = null;
		    self.editing(false);
		}

		// helper function to keep expressions out of markup
		self.getLabel = function( count ) {
			return ko.utils.unwrapObservable( count ) === 1 ? 'item' : 'items';
		};

		// internal computed observable that fires whenever anything changes in our rooms
		ko.computed(function() {
			// store a clean copy to local storage, which also creates a dependency on the observableArray and all observables in each item
			localStorage.setItem('miusiquita-rooms-knockout', ko.toJSON( self.rooms ) );
		}).extend({
			throttle: 500
		}); // save at most twice per second
	};

	var rooms = [];
	

	$.getJSON("/rooms/all-rooms", function (data) {
	    rooms = data;
	    // bind a new instance of our view model to the page
	    var viewModel = new ViewModel(rooms || []);
	    ko.applyBindings(viewModel);

	});
}());
