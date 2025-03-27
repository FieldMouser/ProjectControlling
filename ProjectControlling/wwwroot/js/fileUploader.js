function initFileUploader() {
    const dropZone = document.getElementById('dropZone');
    const fileInput = document.getElementById('projectFiles');
    const fileList = document.getElementById('fileList');
    
    dropZone.addEventListener('click', () => fileInput.click());
    fileInput.addEventListener('change', () => updateFileList());
    
    ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(event => {
        dropZone.addEventListener(event, preventDefaults);
    });

    ['dragenter', 'dragover'].forEach(event => {
        dropZone.addEventListener(event, highlight);
    });

    ['dragleave', 'drop'].forEach(event => {
        dropZone.addEventListener(event, unhighlight);
    });

    dropZone.addEventListener('drop', handleDrop);
    
    function preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    function highlight() {
        dropZone.classList.add('highlight');
    }

    function unhighlight() {
        dropZone.classList.remove('highlight');
    }

    function handleDrop(e) {
        const dt = e.dataTransfer;
        const files = dt.files;
        fileInput.files = files;
        updateFileList();
    }

    function updateFileList() {
        fileList.innerHTML = '';
        Array.from(fileInput.files).forEach(file => {
            const fileItem = document.createElement('div');
            fileItem.className = 'file-item';
            fileItem.innerHTML = `
                <span>${file.name} (${formatFileSize(file.size)})</span>
                <button type="button" class="btn-remove">×</button>
            `;
            fileList.appendChild(fileItem);

            fileItem.querySelector('.btn-remove').addEventListener('click', () => {
                removeFile(file.name);
            });
        });
    }

    function removeFile(filename) {
        const newFiles = Array.from(fileInput.files).filter(f => f.name !== filename);
        const dataTransfer = new DataTransfer();
        newFiles.forEach(f => dataTransfer.items.add(f));
        fileInput.files = dataTransfer.files;
        updateFileList();
    }

    function formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
    }
}