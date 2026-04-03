const PaginationManager = {
    pageSize: 5,
    currentPage: 1,

    update: function (pagedData, fetchFunction) {
        const items = pagedData.items || pagedData.Items || [];
        const totalCount = pagedData.totalCount || pagedData.TotalCount || 0;
        const pageNumber = pagedData.pageNumber || pagedData.PageNumber || 1;
        const pageSize = pagedData.pageSize || pagedData.PageSize || this.pageSize;

        this.currentPage = pageNumber;
        this.pageSize = pageSize;

        const totalPages = Math.ceil(totalCount / pageSize);

        this.renderUI(totalPages, fetchFunction);
        this.updateInfo(totalCount);
    },

    updateInfo: function (total) {
        const info = document.getElementById('paginationInfo');
        if (!info) return;
        const start = (this.currentPage - 1) * this.pageSize + 1;
        const end = Math.min(this.currentPage * this.pageSize, total);
        info.innerText = total > 0 ? `${start}-${end} / Toplam ${total}` : 'Kayıt bulunamadı';
    },

    renderUI: function (totalPages, fetchFunction) {
        const container = document.getElementById('paginationControls');
        if (!container) return;
        container.innerHTML = '';
        if (totalPages <= 1) return;

        let html = `<ul class="pagination pagination-sm mb-0">`;
        html += `<li class="page-item ${this.currentPage === 1 ? 'disabled' : ''}">
                    <a class="page-link" href="javascript:void(0);" onclick="${fetchFunction.name}(${this.currentPage - 1})"><i class="mdi mdi-chevron-left"></i></a>
                 </li>`;

        for (let i = 1; i <= totalPages; i++) {
            html += `<li class="page-item ${i === this.currentPage ? 'active' : ''}">
                        <a class="page-link" href="javascript:void(0);" onclick="${fetchFunction.name}(${i})">${i}</a>
                     </li>`;
        }

        html += `<li class="page-item ${this.currentPage === totalPages ? 'disabled' : ''}">
                    <a class="page-link" href="javascript:void(0);" onclick="${fetchFunction.name}(${this.currentPage + 1})"><i class="mdi mdi-chevron-right"></i></a>
                 </li>`;
        html += `</ul>`;
        container.innerHTML = html;
    }
};