function toggleSidebar() {
    document.querySelector('.shell')?.classList.toggle('collapsed');
}

function toggleGroup(btn) {
    btn.closest('.sb-group').classList.toggle('open');
}

// SetiMarine — Video Hero Controller
window.SetiMarineVideo = {
    init: function () {
        const video = document.getElementById('hero-video');
        if (!video) return;
        video.muted = true;
        video.playsinline = true;
        // Tenta autoplay
        const playPromise = video.play();
        if (playPromise !== undefined) {
            playPromise.catch(() => {
                // Autoplay bloqueado — adiciona listener de interacao
                document.addEventListener('click', () => video.play(), { once: true });
                document.addEventListener('touchstart', () => video.play(), { once: true });
            });
        }
    },
    // Pausa o video quando a aba esta inativa (economia de recursos)
    setupVisibility: function () {
        document.addEventListener('visibilitychange', () => {
            const video = document.getElementById('hero-video');
            if (!video) return;
            if (document.hidden) video.pause();
            else video.play().catch(() => {});
        });
    }
};
document.addEventListener('DOMContentLoaded', function () {
    SetiMarineVideo.init();
    SetiMarineVideo.setupVisibility();
});
