/// <reference path="../../types/window-login.d.ts" />
(function () {
  'use strict';

  function getConfig() {
    return window.loginFormConfig || null;
  }

  function resetLoginUi() {
    window.__loginSubmitting = false;
    var wrap = document.getElementById('progressContainer');
    if (wrap) {
      wrap.classList.remove('is-visible');
      wrap.setAttribute('aria-hidden', 'true');
    }
    var cfg = getConfig();
    if (cfg && cfg.ids && cfg.ids.submit) {
      var btn = document.getElementById(cfg.ids.submit);
      if (btn) btn.classList.remove('is-loading');
    }
  }

  window.addEventListener('pageshow', function (ev) {
    if (ev.persisted) {
      resetLoginUi();
    }
  });

  window.togglePassword = function (inputId) {
    var el = document.getElementById(inputId);
    if (!el) return;
    el.type = el.type === 'password' ? 'text' : 'password';
  };

  window.handleLoginSubmit = function () {
    var cfg = getConfig();
    if (!cfg || !cfg.ids) {
      return true;
    }

    if (window.__loginSubmitting) {
      if (typeof Swal !== 'undefined') {
        Swal.fire({
          icon: 'info',
          title: 'Espere',
          text: cfg.msgReenvio,
          confirmButtonColor: cfg.swalConfirmColor
        });
      }
      return false;
    }

    var userEl = document.getElementById(cfg.ids.user);
    var passEl = document.getElementById(cfg.ids.pass);
    var btn = document.getElementById(cfg.ids.submit);

    var userVal = userEl && userEl.value ? userEl.value.trim() : '';
    var passVal = passEl && passEl.value ? passEl.value.trim() : '';

    if (!userVal) {
      if (typeof Swal !== 'undefined') {
        Swal.fire({
          icon: 'warning',
          title: 'Identificación',
          text: cfg.msgUsuario,
          confirmButtonColor: cfg.swalConfirmColor
        });
      }
      if (userEl) userEl.focus();
      return false;
    }

    if (!passVal) {
      if (typeof Swal !== 'undefined') {
        Swal.fire({
          icon: 'warning',
          title: 'Contraseña',
          text: cfg.msgClave,
          confirmButtonColor: cfg.swalConfirmColor
        });
      }
      if (passEl) passEl.focus();
      return false;
    }

    window.__loginSubmitting = true;

    var wrap = document.getElementById('progressContainer');
    if (wrap) {
      wrap.classList.add('is-visible');
      wrap.setAttribute('aria-hidden', 'false');
    }
    if (btn) btn.classList.add('is-loading');

    return true;
  };
})();
