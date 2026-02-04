<template>
<div class="container-fluid mb-5">
    <div class="left-10">
    <h2 class="text-warning">Search Product</h2>    
    <form class="row g-3" @submit.prevent="submitSearch" autocomplete="off">
        <div class="col-auto">
          <input type="text" required class="form-control-sm" v-model="search" name="search" placeholder="enter Product keyword">
          <div class="text-warning">{{message}}</div>
        </div>
        <div class="col-auto">
          <button type="submit" class="btn btn-primary btn-sm mb-3">search</button>
        </div>
    </form>
    </div>
    <div class="container-fluid mb-4">
        <div class="card-group">
            <div v-for="prod in prodsearch" :key="prod['id']" class="col-md-4">
                <div class="card mx-3 mt-2">
                    <img v-bind:src="`http://localhost:5094/products/${prod['productpicture']}`" class="card-img" alt=""/>
                    <div class="card-body cardbody-height">
                        <h5 class="card-title">Description</h5>
                        <p class="card-text">{{ prod['descriptions'] }}</p>
                    </div>
                    <div class="card-footer price-size">
                        <p class="card-text text-danger"><span class="text-dark">PRICE :</span>&nbsp;<strong>&#8369;{{ toDecimal(prod['sellprice']) }}</strong></p>
                    </div>  
                </div>
            </div>
        
        </div>    
    </div>

    <div class="left-10" v-if="totpage > 1">
        <nav aria-label="Page navigation example">
            <ul class="pagination">
              <li class="page-item"><a @click="lastPage($event)" class="page-link" href="#">Last</a></li>
              <li class="page-item"><a @click="prevPage($event)" class="page-link" href="#">Previous</a></li>
              <li class="page-item"><a @click="nextPage" class="page-link" href="#">Next</a></li>
              <li class="page-item"><a @click="firstPage($event)" class="page-link" href="#">First</a></li>
              <li class="page-item page-link text-danger">Page&nbsp;{{ pages }} of&nbsp;{{ totpage }}</li>
            </ul>
          </nav>
          <div class="tot-rec text-white">Total Records : {{ totalrecords }}</div>
        </div>  

</div>

</template>

<style scoped>
.card-body {
    height: 15vh !important;
}

</style>


<script setup lang="ts">
import { ref } from 'vue';
import axios from 'axios';

const api = axios.create({
    baseURL: "http://localhost:5094",
    headers: {'Accept': 'application/json',
            'Content-Type': 'application/json'}
});

const search = ref<string>('');
const prodsearch = ref([]);
const message = ref<string>('');
const totalrecords = ref<number>(0);
const pages = ref<number>(1);
const totpage = ref<number>(0);


const getProdsearch = async (page: number, keyword: string) => {
            if (totpage.value === 0) {
                message.value = "searching...";
            }

            const productsPayload = {
                query: `
                query ProductSearch($input: KeyInput!) {
                    searchProducts(input: $input) {
                    products {
                      id
                      descriptions
                      qty
                      unit
                      costprice
                      sellprice
                      productpicture
                    }
                    page
                    totalpage
                    totalrecords
                  }
                }
                `,
                variables: {
                    input: {
                        page: page,
                        keyword: keyword  
                    }
                }
            };

            try {
                const res = await api.post('/graphql', productsPayload); 
                
                if (res.data.errors) {
                    message.value = res.data.errors[0].message;
                    setTimeout(() => {
                      message.value = '';
                      prodsearch.value = [];
                      totalrecords.value = 0;
                      totpage.value = 0;
                      
                    }, 3000);
                    return;
                } else {
                    setTimeout(() => { message.value = ''; }, 1000);
                    const result = res.data.data?.searchProducts; 
                    prodsearch.value = result.products;
                    totalrecords.value = result.totalrecords;
                    pages.value = Number(result.page);
                    totpage.value = result.totalpage;
                }
            } catch (error: any) {
                message.value = error.response?.data?.errors?.[0]?.message || error.message || "An error occurred";
                setTimeout(() => { message.value = ''; prodsearch.value = []; totalrecords.value = 0;}, 3000);

            } finally {
            }
}

const submitSearch = () => {
    getProdsearch(pages.value, search.value);
}

const nextPage = (event: any) => {
      event.preventDefault();
      if (pages.value == totpage.value) {
          return;
      }
      pages.value++;
      return getProdsearch(pages.value, search.value);
    }


    const prevPage = (event: any) => {
      event.preventDefault();
      if (pages.value == 1) {
      return;
      }
      pages.value--;
      return getProdsearch(pages.value, search.value);
    }

    const firstPage = (event: any) => {
      event.preventDefault();
      pages.value = 1;
      return getProdsearch(pages.value, search.value);
    }

    const lastPage = (event: any) => {
      event.preventDefault();
      pages.value = totpage.value;
      return getProdsearch(pages.value, search.value);
    }

    const toDecimal = (nos: number) => {
      const formatter = new Intl.NumberFormat('en-US', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      });
      return formatter.format(nos);
    }
    

</script>