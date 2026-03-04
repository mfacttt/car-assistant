let dairyNewFiles = [];

function resetDairyState() {
    dairyNewFiles = [];
}

function openModal(mode, postId = null) {
    console.log('[dairy] openModal called', { mode, postId });
    const modal = document.getElementById('modalOverlay');
    const title = document.getElementById('modalTitle');
    const idInput = document.getElementById('postId');

    const container = document.getElementById('photoPreviewContainer');
    container.innerHTML = '';
    resetDairyState();

    if (mode === 'new') {
        title.textContent = 'Новая запись';
        document.getElementById('postTitle').value = '';
        document.getElementById('postText').value = '';
        if (idInput) {
            idInput.value = '';
        }
    } else if (postId != null) {
        title.textContent = 'Редактировать запись';
        const card = document.querySelector('[data-entry-id="' + postId + '"]');
        if (card) {
            const entryTitle = card.getAttribute('data-entry-title') || '';
            const entryText = card.getAttribute('data-entry-text') || '';
            document.getElementById('postTitle').value = entryTitle;
            document.getElementById('postText').value = entryText;
            if (idInput) {
                idInput.value = String(postId);
            }

            // Заполнить превью уже сохранёнными фото
            const images = card.querySelectorAll('img[data-photo-url]');
            images.forEach(img => {
                const url = img.getAttribute('data-photo-url');
                if (!url) return;

                const wrapper = document.createElement('div');
                wrapper.className = 'relative inline-block';
                wrapper.dataset.existingPhotoUrl = url;
                wrapper.innerHTML = `
                    <img src="${url}" class="h-16 w-16 object-cover rounded-lg border border-purple-500/30">
                    <button type="button" class="absolute -top-1 -right-1 w-5 h-5 rounded-full bg-black/70 text-white text-xs flex items-center justify-center" onclick="removeDairyExistingPhoto('${encodeURIComponent(url)}')">&times;</button>
                    <input type="hidden" name="existingPhotoUrls" value="${url}">
                `;
                container.appendChild(wrapper);
            });
        }
    }

    modal.classList.add('active');
    console.log('[dairy] modal opened');
}

function closeModal(event = null) {
    if (event && event.target !== document.getElementById('modalOverlay') && !event.target.classList.contains('modal-overlay')) {
        return;
    }

    document.getElementById('modalOverlay').classList.remove('active');
    console.log('[dairy] modal closed');
}

function ensureDairyFileInput() {
    let input = document.getElementById('photosInput');
    if (input) {
        console.log('[dairy] ensureDairyFileInput: existing input found');
        return input;
    }

    const previewContainer = document.getElementById('photoPreviewContainer');
    console.log('[dairy] ensureDairyFileInput: creating new input', previewContainer);

    input = document.createElement('input');
    input.type = 'file';
    input.id = 'photosInput';
    input.name = 'photos';
    input.multiple = true;
    input.accept = 'image/*';
    input.style.position = 'absolute';
    input.style.left = '-9999px';
    input.style.width = '1px';
    input.style.height = '1px';
    input.style.opacity = '0';

    input.addEventListener('change', function () {
        console.log('[dairy] photosInput change event fired');
        handleDairyPhotosSelected(input);
    });

    if (previewContainer && previewContainer.parentElement) {
        previewContainer.parentElement.appendChild(input);
    } else {
        document.body.appendChild(input);
    }

    return input;
}

function openDairyFileDialog() {
    console.log('[dairy] openDairyFileDialog called');
    const input = ensureDairyFileInput();
    console.log('[dairy] photosInput element (after ensure)', input);
    if (input) {
        console.log('[dairy] calling input.click()');
        input.click();
    }
}

// Для совместимости со старой разметкой, где использовался onclick="addPhotoInput()"
function addPhotoInput() {
    console.log('[dairy] addPhotoInput called');
    openDairyFileDialog();
}

function handleDairyPhotosSelected(input) {
    console.log('[dairy] handleDairyPhotosSelected called', { filesCount: input && input.files ? input.files.length : null });
    const container = document.getElementById('photoPreviewContainer');

    if (!input.files || input.files.length === 0) {
        return;
    }

    Array.from(input.files).slice(0, 10).forEach(file => {
        if (!file.type.startsWith('image/')) {
            return;
        }

        const url = URL.createObjectURL(file);
        const wrapper = document.createElement('div');
        wrapper.className = 'relative inline-block';

        const id = 'new_' + Date.now() + '_' + Math.random().toString(36).slice(2);
        wrapper.dataset.newPhotoId = id;

        dairyNewFiles.push({ id, file });

        wrapper.innerHTML = `
            <img src="${url}" class="h-16 w-16 object-cover rounded-lg border border-purple-500/30">
            <button type="button" class="absolute -top-1 -right-1 w-5 h-5 rounded-full bg-black/70 text-white text-xs flex items-center justify-center" onclick="removeDairyNewPhoto('${id}')">&times;</button>
        `;
        container.appendChild(wrapper);
    });
    // очищаем input, чтобы можно было выбрать те же файлы ещё раз при необходимости
    input.value = '';
    console.log('[dairy] previews rendered');
}

function removeDairyExistingPhoto(encodedUrl) {
    const url = decodeURIComponent(encodedUrl);
    const container = document.getElementById('photoPreviewContainer');
    if (!container) return;

    const wrapper = container.querySelector('[data-existing-photo-url="' + url.replace(/"/g, '\"') + '"]');
    if (wrapper) {
        wrapper.remove();
    }
}

function removeDairyNewPhoto(id) {
    dairyNewFiles = dairyNewFiles.filter(x => x.id !== id);
    const container = document.getElementById('photoPreviewContainer');
    if (!container) return;

    const wrapper = container.querySelector('[data-new-photo-id="' + id + '"]');
    if (wrapper) {
        wrapper.remove();
    }
}

// ======= Просмотр фото во весь экран =======
let dairyViewerState = {
    urls: [],
    index: 0
};

function openDairyPhotoViewer(img) {
    const card = img.closest('[data-entry-id]');
    if (!card) return;

    const images = card.querySelectorAll('img[data-photo-url]');
    dairyViewerState.urls = Array.from(images).map(i => i.getAttribute('data-photo-url'));
    dairyViewerState.index = dairyViewerState.urls.indexOf(img.getAttribute('data-photo-url'));

    const viewer = document.getElementById('dairyPhotoViewer');
    const viewerImg = document.getElementById('dairyViewerImage');
    if (!viewer || !viewerImg || dairyViewerState.index < 0) return;

    viewerImg.src = dairyViewerState.urls[dairyViewerState.index];
    viewer.classList.remove('hidden');
}

function closeDairyPhotoViewer(event) {
    if (event) {
        const viewer = document.getElementById('dairyPhotoViewer');
        if (!viewer) return;

        viewer.classList.add('hidden');
    }
}

function showDairyPhotoAt(indexDelta) {
    if (!dairyViewerState.urls.length) return;

    dairyViewerState.index = (dairyViewerState.index + indexDelta + dairyViewerState.urls.length) % dairyViewerState.urls.length;
    const viewerImg = document.getElementById('dairyViewerImage');
    if (viewerImg) {
        viewerImg.src = dairyViewerState.urls[dairyViewerState.index];
    }
}

function nextDairyPhoto(event) {
    event.stopPropagation();
    showDairyPhotoAt(1);
}

function prevDairyPhoto(event) {
    event.stopPropagation();
    showDairyPhotoAt(-1);
}

function deleteDairyEntry(id) {
    const formData = new FormData();
    formData.append('id', id);

    fetch('/Dairy/Delete', {
        method: 'POST',
        body: formData
    }).then(response => {
        if (response.redirected) {
            window.location.href = response.url;
        } else {
            window.location.reload();
        }
    }).catch(error => {
        console.error('[dairy] error deleting entry', error);
    });
}
// ======= Свайпы для карточек дневника (удаление по свайпу, как в уведомлениях) =======
let dairyCurrentItem = null;
let dairyStartX = 0;
let dairyCurrentX = 0;
let dairyIsSwiping = false;
const dairySwipeThreshold = 40;

function startSwipe(event) {
    const clientX = event.type.startsWith('touch') ? event.touches[0].clientX : event.clientX;
    const item = event.target.closest('.notification-item');
    if (!item) return;

    document.querySelectorAll('.notification-item.swiped').forEach(element => {
        element.classList.remove('swiped');
    });

    dairyCurrentItem = item;
    dairyStartX = clientX;
    dairyIsSwiping = true;
}

function swipeMove(event) {
    if (!dairyIsSwiping || !dairyCurrentItem) return;

    const clientX = event.type.startsWith('touch') ? event.touches[0].clientX : event.clientX;
    const deltaX = clientX - dairyStartX;

    if (deltaX < 0 && Math.abs(deltaX) > 10) {
        event.preventDefault();
        const translateX = Math.max(-60, deltaX);
        dairyCurrentItem.style.transform = `translateX(${translateX}px)`;
        dairyCurrentX = translateX;
    }
}

function endSwipe() {
    if (!dairyIsSwiping || !dairyCurrentItem) {
        resetSwipe();
        return;
    }

    if (dairyCurrentX < -dairySwipeThreshold) {
        dairyCurrentItem.classList.add('swiped');
        dairyCurrentItem.style.transform = '';

        const container = dairyCurrentItem.closest('.notification-container');
        if (container) {
            const deleteBtn = container.querySelector('.delete-btn');
            if (deleteBtn) {
                deleteBtn.classList.add('visible');
            }
        }
    } else {
        dairyCurrentItem.style.transform = '';
    }

    resetSwipe();
}

function cancelSwipe() {
    if (dairyCurrentItem) {
        dairyCurrentItem.style.transform = '';
    }

    resetSwipe();
}

function resetSwipe() {
    dairyCurrentItem = null;
    dairyStartX = 0;
    dairyCurrentX = 0;
    dairyIsSwiping = false;
}

// Swipe navigation for touch devices (просмотр фото)
document.addEventListener('DOMContentLoaded', function () {
    console.log('[dairy] DOMContentLoaded');
    const photosInput = ensureDairyFileInput();
    console.log('[dairy] photosInput on DOMContentLoaded (after ensure)', photosInput);

    const form = document.getElementById('postForm');
    if (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();

            const formData = new FormData();

            const idInput = document.getElementById('postId');
            const titleInput = document.getElementById('postTitle');
            const textInput = document.getElementById('postText');

            if (idInput && idInput.value) {
                formData.append('Id', idInput.value);
            }
            if (titleInput) {
                formData.append('Title', titleInput.value || '');
            }
            if (textInput) {
                formData.append('Text', textInput.value || '');
            }

            // Существующие фото, которые пользователь оставил
            document.querySelectorAll('#photoPreviewContainer input[name="existingPhotoUrls"]').forEach(input => {
                if (input.value) {
                    formData.append('existingPhotoUrls', input.value);
                }
            });

            // Новые фото
            dairyNewFiles.forEach(item => {
                formData.append('photos', item.file);
            });

            fetch(form.action, {
                method: 'POST',
                body: formData
            }).then(response => {
                if (response.redirected) {
                    window.location.href = response.url;
                } else {
                    window.location.reload();
                }
            }).catch(error => {
                console.error('[dairy] error submitting form', error);
            });
        });
    }

    document.addEventListener('click', function (event) {
        if (!event.target.closest('.notification-container')) {
            document.querySelectorAll('.notification-item.swiped').forEach(element => {
                element.classList.remove('swiped');
            });

            document.querySelectorAll('.delete-btn.visible').forEach(element => {
                element.classList.remove('visible');
            });
        }
    });

    const viewer = document.getElementById('dairyPhotoViewer');
    if (!viewer) return;

    let touchStartX = null;

    viewer.addEventListener('touchstart', function (e) {
        if (e.touches.length === 1) {
            touchStartX = e.touches[0].clientX;
        }
    }, { passive: true });

    viewer.addEventListener('touchend', function (e) {
        if (touchStartX === null) return;

        const deltaX = e.changedTouches[0].clientX - touchStartX;
        const threshold = 50; // минимальное смещение для свайпа

        if (Math.abs(deltaX) > threshold) {
            if (deltaX < 0) {
                showDairyPhotoAt(1); // свайп влево – следующее фото
            } else {
                showDairyPhotoAt(-1); // свайп вправо – предыдущее фото
            }
        }

        touchStartX = null;
    }, { passive: true });
});
