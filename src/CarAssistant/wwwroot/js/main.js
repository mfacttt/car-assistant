"use strict";

function setAppLanguage(lang) {
	try {
		document.cookie = "app-lang=" + encodeURIComponent(lang) + "; path=/; max-age=" + (365 * 24 * 60 * 60);
	} catch (e) {
		console.warn("Cannot set language cookie", e);
	}

	const buttons = document.querySelectorAll("button[data-lang]");
	buttons.forEach(btn => {
		if (btn.getAttribute("data-lang") === lang) {
			btn.classList.remove("text-slate-400");
			btn.classList.add("text-slate-100", "bg-purple-600/40");
		} else {
			btn.classList.remove("text-slate-100", "bg-purple-600/40");
			btn.classList.add("text-slate-400");
		}
	});
}

document.addEventListener("DOMContentLoaded", function () {
	const match = document.cookie.match(/(?:^|; )app-lang=([^;]+)/);
	const lang = match ? decodeURIComponent(match[1]) : "ru";
	setAppLanguage(lang);
});
