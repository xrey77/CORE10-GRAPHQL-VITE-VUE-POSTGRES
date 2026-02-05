<template>
 <vue3-html2pdf
    ref="html2Pdf"
    :show-layout="false"
    :float-layout="true"
    :enable-download="false"
    :preview-modal="true"
    :manual-pagination="true"
    :html-to-pdf-options="pdfOptions"
    :paginate-elements-by-height="1400"
    filename="product-report"
    pdf-format="a4"
    @beforeDownload="beforeDownload($event)"
    @hasGenerated="onGenerated"
  >
    <template #pdf-content>
      <section class="pdf-container" slot="pdf-content">
        <div style="display: flex; justify-content: center; width: 100%; margin-top: -10px;">
          <img class="w-100 img-center" v-bind:src="logoUrl" alt=""/>
        </div>
        <h1 class="text-center text-black">Products Report</h1>
        <table class="table">
          <thead>
            <tr>
              <th class="bg-primary text-white">#</th>
              <th class="bg-primary text-white">Descriptions</th>
              <th class="bg-primary text-white">Stocks</th>
              <th class="bg-primary text-white">Unit</th>
              <th class="bg-primary text-white">Price</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in productsdata" :key="item.id">
              <td>{{ item.id }}</td>
              <td>{{ item.descriptions }}</td>
              <td>{{ item.qty }}</td>
              <td>{{ item.unit }}</td>
              <td>₱{{ toDecimal(item.sellprice) }}</td>
            </tr>
          </tbody>
        </table>
      </section>

    </template>
  </vue3-html2pdf>
</template>

<style lang="css">
.pdf-footer {
    page-break-before: always;
}
.img-center {
  width: 50px !important;
  height: 50px !important;
}
</style>




<script setup lang="ts">
import axios from 'axios';
import { ref, nextTick } from 'vue';
import logoUrl from "../assets/logo.svg";
import Vue3Html2pdf from 'vue3-html2pdf'

const productsdata = ref<any[]>([]);
const message = ref<string>('');
const html2Pdf = ref<any>(null);

const api = axios.create({
    baseURL: "http://localhost:5094",
    headers: {'Accept': 'application/json',
                'Content-Type': 'application/json'}
});

const generateReport = async () => {
  if (productsdata.value.length === 0) {
    await getProductsData();
  }
  await nextTick();
  html2Pdf.value?.generatePdf();
};


const getProductsData = async () => {
    const productsData = {
        query: `
        query ProductReport {
           pdfReport {
             id
             descriptions
             qty
             unit
             costprice
             sellprice
             productpicture
            }  
        }
        `
    };

    try {
        const res = await api.post('/graphql', productsData); 
        
        if (res.data.errors) {
            message.value = res.data.errors[0].message;
            setTimeout(() => {
                message.value = '';
                
            }, 3000);
            return;
        } else {
            const result = res.data.data; 
            productsdata.value = result.pdfReport;
        }
    } catch (error: any) {
        message.value = error.response?.data?.errors?.[0]?.message || error.message || "An error occurred";
    } finally {
        setTimeout(() => { message.value = ''; }, 3000);
    }
}

const onGenerated = (blob: Blob) => {
  viewPdf(blob);
  console.log('PDF Generated successfully', blob);
};


const toDecimal = (nos: number) => {
    const formatter = new Intl.NumberFormat('en-US', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
    });
    return formatter.format(nos);
}

const pdfOptions = {
  margin: [20, 10, 25, 10], // Top, Right, Bottom, Left margin in mm  
  pagebreak: { mode: ['css', 'legacy'] },  
  image: { type: 'jpeg', quality: 0.98 },
//   html2canvas: { scale: 2, scrollY: 0 },
  html2canvas: { 
    scale: 2, 
    scrollY: 0, 
    useCORS: true, // Crucial for rendering images
    allowTaint: true 
  },
  jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' }
};

const beforeDownload = async ({ html2pdf, options, pdfContent }: any) => {
  await html2pdf()
    .set(options)
    .from(pdfContent)
    .toPdf()
    .get('pdf')
    .then((pdf: any) => {
      const totalPages = pdf.internal.getNumberOfPages();
      
      for (let i = 1; i <= totalPages; i++) {
        pdf.setPage(i);
        pdf.setFontSize(10);
        pdf.setTextColor(150);
        
        // Coordinates for A4: Width ~210mm, Height ~297mm
        const xPos = pdf.internal.pageSize.getWidth() / 2; // Center
        const yPos = pdf.internal.pageSize.getHeight() - 10; // 10mm from bottom
        
        pdf.text(`Page ${i} of ${totalPages}`, xPos, yPos, { align: 'center' });
      }
    })
    .save(); // Manually trigger download after modification
};

const viewPdf = (blobFile: Blob) => {
  const blobUrl = URL.createObjectURL(blobFile);
  window.open(blobUrl, '_blank');
};

generateReport();


</script>

