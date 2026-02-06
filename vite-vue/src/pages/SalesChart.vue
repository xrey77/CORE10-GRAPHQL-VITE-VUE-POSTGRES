<template>
  <main>
    <div class="container container-top">

      <div class="div-width">
        <p class="text-center text-white">Annual Sales Report</p>      
      </div>
      <Bar v-if="loaded" :data="chartData" :options="chartOptions" :plugins="[logoPlugin]"   />
      <div v-else class="text-white">Loading...</div>            
    </div>
  </main>
</template> 

<style lang="css">
    .container-top {
        margin-top:  -50px !important;
    }
  .div-width {
    max-width: 100% !important;
    width: 100% !important;
    align-content: center !important;
  }
</style>

<script setup lang="ts">
import axios from 'axios';
import { ref, onMounted } from 'vue';
import { Bar } from 'vue-chartjs';
import { 
  Chart as ChartJS, Title, Tooltip, Legend, 
  BarElement, CategoryScale, LinearScale 
} from 'chart.js';
import logoUrl from '../../public/images/logo.png';

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale);

const logoImage = new Image();
logoImage.src = logoUrl;

const logoPlugin = {
  id: 'custom_canvas_logo',
  afterDraw: (chart: any) => {
    if (logoImage.complete) {
      const { ctx, width } = chart; // full canvas width
      
      const logoWidth = 150; 
      const logoHeight = 50;
      
      // Calculate 'x' for horizontal centering on the whole canvas
      const x = (width / 2) - (logoWidth / 2);
      // Set 'y' to a small offset from the absolute top (e.g., 10px)
      const y = 10; 
      
      ctx.drawImage(logoImage, x, y, logoWidth, logoHeight);
    } else {
      logoImage.onload = () => chart.draw();
    }
  }  
};

const chartOptions = {
  responsive: true,
  layout: {
    // Add top padding so the chart elements (legend/title) 
    // don't start until below your 50px logo
    padding: {
      top: 70 
    }
  },
  plugins: {
    legend: {
      labels: { color: 'white' }
    }
  },
  scales: {
    x: { ticks: { color: 'white' } },
    y: { ticks: { color: 'white' } }
  }
};

const api = axios.create({
    baseURL: "http://localhost:5094",
    headers: { 'Accept': 'application/json', 'Content-Type': 'application/json' }
});

const loaded = ref(false);
const chartData = ref<any>({
  labels: [],
  datasets: []
});

const getSales = async () => {
    const graphqlQuery = {
        query: `query ChartData { salesdata { amount monthdate } }`
    };

    try {
        const res = await api.post('/graphql', graphqlQuery); 
        const rawData = res.data.data.salesdata; 

        chartData.value = {
            labels: rawData.map((item: any) => {
                    const date = new Date(item.monthdate);
                    return new Intl.DateTimeFormat('en-US', { month: 'short' }).format(date);
                }),
            datasets: [{
                label: 'Monthly Sales',
                backgroundColor: '#79F527',
                data: rawData.map((item: any) => item.amount)
            }]
        };
        
        loaded.value = true;
    } catch (error: any) {
        console.error("Fetch error:", error);
    }
};

onMounted(getSales);
</script>
