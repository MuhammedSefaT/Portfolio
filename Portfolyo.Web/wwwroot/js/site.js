// Bölümler görünür alana girdiğinde bir kez belirir.
// Hareket azaltma tercihi açıksa hiç çalışmaz; CSS zaten içeriği görünür bırakır.
(function () {
    'use strict';

    var azaltilmisHareket = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    var bolumler = document.querySelectorAll('[data-reveal]');

    if (!bolumler.length) {
        return;
    }

    // IntersectionObserver yoksa (eski tarayıcı) içerik doğrudan gösterilir.
    if (azaltilmisHareket || !('IntersectionObserver' in window)) {
        bolumler.forEach(function (bolum) {
            bolum.classList.add('is-visible');
        });
        return;
    }

    var gozlemci = new IntersectionObserver(function (girisler) {
        girisler.forEach(function (giris) {
            if (!giris.isIntersecting) {
                return;
            }

            giris.target.classList.add('is-visible');
            gozlemci.unobserve(giris.target);
        });
    }, {
        rootMargin: '0px 0px -12% 0px',
        threshold: 0.08
    });

    bolumler.forEach(function (bolum) {
        gozlemci.observe(bolum);
    });
})();
