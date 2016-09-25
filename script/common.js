var Form;
var Message;
var Search;

/*------------------------------------------------------------------------
	function call included in template for all body tags

	Date:		Name:	Description:
	11/25/04	JEA		Creation	
------------------------------------------------------------------------*/
function BodyLoad() {
	Form = document.forms[0];
	PreFetchImages();
	
	WebPart = new WebPartClass();
	Search = new SearchClass();
	Message = new MessageClass();
	
	WebPart.LoadPreferences();
	Message.Show();
}

/*------------------------------------------------------------------------
	give time for page to render then redirect, usually for download

	Date:		Name:	Description:
	1/23/05		JEA		Creation	
------------------------------------------------------------------------*/
function Redirect(url) {
	AddEvent(window, "load", setTimeout("location.href=\"" + url + "\"", 500));
}

/*------------------------------------------------------------------------
	handle button rollover

	Date:		Name:	Description:
	11/25/04	JEA		Creation	
------------------------------------------------------------------------*/
function Button(img) {
	var image = (typeof(img.style.filter) == "string") ? img.style.filter : img.src;
	var re = /_on\.(png|gif|jpg)/;
	if (re.test(image)) {		// turn off
		var changeTo = ".$1";
	} else {					// turn on
		var changeTo = "_on.$1";
		re = /\.(png|gif|jpg)/;
	}
	if (typeof(img.style.filter) == "string") {
		img.style.filter = image.replace(re, changeTo);
	} else {
		img.src = image.replace(re, changeTo);
	}
}

/*------------------------------------------------------------------------
	cache mouse-over images

	Date:		Name:	Description:
	1/1/03		JEA		Creation
	11/28/04	JEA		Support other image formats
	1/24/05		JEA		Handle input images
------------------------------------------------------------------------*/
function PreFetchImages() {
	var _cached = new Array();
	var _re = /\.(png|gif|jpg)/gi;
	var _images = document.getElementsByTagName('img');
	var _input = document.getElementsByTagName('input');

	for (var x = 0; x < _images.length; x++) { CacheSrc(_images[x]); } 
	for (var x = 0; x < _input.length; x++) {
		if (_input[x].getAttribute('type') == "image") { CacheSrc(_input[x]); }
	}
	function CacheSrc(img) {
		var imagePath = null;
		if (typeof(img.style.filter) == "string") {
			var re = /src=[\'\"](.*)[\'\"],/i;
			var matches = re.exec(img.style.filter);
			if (matches != null) { imagePath = matches[1]; }
		} else {
			imagePath = img.getAttribute('src');
		}
		if (imagePath != null) {
			var imageName = imagePath.substring(imagePath.lastIndexOf("/") + 1, imagePath.length);
			if (imageName.substr(0,4) == "btn_" && imageName.indexOf("_on.") == -1) {
				_cached.push(new Image());
				_cached[_cached.length - 1].src = imagePath.replace(_re, "_on.$1");
			}
		}
	}
}

/*------------------------------------------------------------------------
	clear selections from option list

	Date:		Name:	Description:
	1/5/03		JEA		Creation
------------------------------------------------------------------------*/
function ClearSelection(id) {
	var field = Form.elements[id];
	for (var x = 0; x < field.options.length; x++) {
		field.options[x].selected = false;
	}
}

/*------------------------------------------------------------------------
	generic method to add a listener for an object event

	Date:		Name:	Description:
	1/3/05		JEA		Creation
------------------------------------------------------------------------*/
function AddEvent(obj, evType, fn) {
	if (obj.addEventListener) {
		obj.addEventListener(evType, fn, false);
		return true;
	} else if (obj.attachEvent) {
		var r = obj.attachEvent('on'+evType, fn);
		return r;
	} else {
		return false;
	}
}

/*------------------------------------------------------------------------
	perform search

	Date:		Name:	Description:
	11/25/04	JEA		Creation	
------------------------------------------------------------------------*/
function SearchClass() {
	var me = this;
	var _tag = document.getElementById("fldSearch");
	
	_tag.focus();
	//_tag.onkeydown = me.CheckKey;
	
	this.Execute = function() {
		var text = _tag.value.replace(/\s+$/,"");
		if (text != "") {
			location.href = "search.aspx?text=" + escape(text);
		}		
	}
	this.CheckKey = function(e) {
		if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
			e.returnValue = false; e.cancel = true; me.Execute();
		}
	}
	this.Focus = function() {
		_tag.focus();
	}
}

/*------------------------------------------------------------------------
	fade text in div tag

	Date:		Name:	Description:
	12/7/04		JEA		Creation
------------------------------------------------------------------------*/
function MessageClass() {
	var _opacity;
	var _timer;
	var _tag = document.getElementById("message")
	
	this.Text = function(text) {
		if (text.length > 0) { _tag.value = text; }
		return _tag.value;
	}
	this.Show = function() {
		if (_tag != undefined) { setTimeout(BeginFade, 3000); }
	}
	function BeginFade() {
		_opacity = 0.999;		// 1.0 causes flash in FF
		_timer = setInterval(Fade, 50);
	}
	function Fade() {
		if (_opacity > 0) {
			SetOpacity(_tag, _opacity);
			_opacity -= 0.01;
		} else {
			_tag.style.display = "none";
			_opacity = 1;
			clearInterval(_timer);
		}
	}
	// http://www.sitepoint.com/blog-post-view.php?id=211431
	function SetOpacity(tag, level) {
		with (tag.style) {
			filter = "alpha(opacity:" + (100 * level) + ")";	// IE
			KHTMLOpacity = level;								// Konqueror, old Safari
			MozOpacity = level;									// old Mozilla
			opacity = level;									// W3C
		}
	}
}