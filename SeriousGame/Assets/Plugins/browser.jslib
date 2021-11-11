mergeInto(LibraryManager.library, {
  Redirect: function (url) {
	window.open(Pointer_stringify(url), '_self');
  }
});
